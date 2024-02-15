using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Models;
using PaymentManager.Common.Constants;
using PaymentManager.DAL.Enums;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;
using Stripe;
using Stripe.Checkout;

namespace PaymentManager.BLL.Services;

/// <summary>
///     This class contains methods for payment session related operations
/// </summary>
public class PaymentSessionService : StripeBaseService, IPaymentSessionService
{
    private const int AmountOfItems = 1;
    private readonly IPaymentCustomerService _paymentCustomerService;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IProductCatalogService _productCatalogService;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserRepository _userRepository;

    public PaymentSessionService(IPaymentRepository paymentRepository,
        IUserRepository userRepository,
        IProductCatalogService productCatalogService,
        IPaymentCustomerService paymentCustomerService,
        ISubscriptionRepository subscriptionRepository)
    {
        _paymentCustomerService = paymentCustomerService;
        _productCatalogService = productCatalogService;
        _userRepository = userRepository;
        _paymentRepository = paymentRepository;
        _subscriptionRepository = subscriptionRepository;
    }

    /// <inheritdoc cref="IPaymentSessionService.CreateOneTimePaymentAsync(Guid, CreatePaymentRequest)" />
    public async Task<SessionCreateOptions> CreateOneTimePaymentAsync(Guid userId, CreatePaymentRequest streamRequest)
    {
        var user = await _userRepository.GetUserByIdAsync(userId)
                   ?? throw new ArgumentNullException($"User with id - {userId} was not found");

        await IfUserAbleToUsePaymentAsync(user);

        var sessionCreateOptions = new SessionCreateOptions
        {
            SuccessUrl = streamRequest.SuccessUrl,
            CancelUrl = streamRequest.CancelUrl,
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = Constants.ApplicationCurrency,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"{streamRequest.AmountOfPoints} - points"
                        },
                        UnitAmountDecimal = streamRequest.AmountOfPoints * Constants.OneTokenPrice *
                                            Constants.PriceInCents
                    },
                    Quantity = AmountOfItems
                }
            },
            PaymentIntentData = new SessionPaymentIntentDataOptions
                { Description = $"{streamRequest.AmountOfPoints} - points" },
            Mode = Constants.PaymentMode,
            Customer = string.IsNullOrEmpty(user.StripeCustomerId) ? null : user.StripeCustomerId,
            CustomerEmail = string.IsNullOrEmpty(user.StripeCustomerId) ? user.Email : null,
            Metadata = new Dictionary<string, string>
            {
                { MetadataConstants.ProductName, $"{streamRequest.AmountOfPoints} - points" },
                { MetadataConstants.AmountOfPoint, streamRequest.AmountOfPoints.ToString() },
                { MetadataConstants.UserId, user.Id.ToString() },
                { MetadataConstants.ConnectionId, streamRequest.ConnectionId },
                { MetadataConstants.SignalRMethodName, streamRequest.SignalMethodName }
            }
        };

        return sessionCreateOptions;
    }

    /// <inheritdoc cref="IPaymentSessionService.GetUserBalanceAsync(Guid)" />
    public async Task<int> GetUserBalanceAsync(Guid userId)
    {
        var userWallet = await _paymentRepository.GetUserWalletAsync(userId)
                         ?? throw new Exception($"We can`t find wallet by userId => {userId}");

        return userWallet.Balance;
    }

    /// <inheritdoc cref="IPaymentSessionService.CreateFreeSubscriptionAsync(SignalRModel, Guid)" />
    public async Task<SubscriptionCreateOptions> CreateFreeSubscriptionAsync(SignalRModel signalRModel, Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId)
                   ?? throw new ArgumentNullException(nameof(userId));

        if (user.AccountPlan is not null)
            throw new Exception("You already have subscription");

        var product = await _productCatalogService.GetProductByNameAsync(AccountPlan.Free.ToString());

        await IfUserAbleToUsePaymentAsync(user, true);

        return new SubscriptionCreateOptions
        {
            Customer = user.StripeCustomerId,
            Items = new List<SubscriptionItemOptions> { new() { Price = product.PriceId } },
            Metadata = new Dictionary<string, string>
            {
                { MetadataConstants.AccountPlan, AccountPlan.Free.ToString() },
                { MetadataConstants.UserId, user.Id.ToString() },
                { MetadataConstants.ProductName, product.Name },
                { MetadataConstants.ConnectionId, signalRModel.ConnectionId },
                { MetadataConstants.SignalRMethodName, signalRModel.SignalMethodName }
            }
        };
    }

    /// <inheritdoc cref="IPaymentSessionService.CancelSubscriptionAsync(Guid)" />
    public async Task CancelSubscriptionAsync(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId)
                   ?? throw new ArgumentNullException(nameof(userId));

        var customerData = await GetCustomerService().GetAsync(user.StripeCustomerId, new CustomerGetOptions
        {
            Expand = new List<string> { "subscriptions" }
        }) ?? throw new Exception("We can`t find you payment profile");

        if (customerData.Subscriptions?.Any() != true)
            throw new Exception("You don`t have any subscription");

        var getProductId = customerData.Subscriptions.First().Items.Data.FirstOrDefault()?.Plan.ProductId
                           ?? throw new Exception("Taking productId error");

        var productModel = await _productCatalogService.GetProductAsync(getProductId);
        var productName = productModel.Name[..productModel.Name.IndexOf("/", StringComparison.Ordinal)];

        if (productName.Equals(AccountPlan.Free.ToString()))
            throw new Exception("You can`t cancel Free subscription");

        await GetSubscriptionService().UpdateAsync(customerData.Subscriptions.FirstOrDefault()?.Id,
            new SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = true
            });

        await _subscriptionRepository.UpdateSubscriptionStatusAsync(customerData.Subscriptions.First().Id,
            SubscriptionStatusEnum.PendingCancellation);
    }

    public async Task UpgradeSubscriptionAsync(Guid userId, string newPriceId)
    {
        var getActiveSubscription = await _paymentCustomerService.GetActiveSubscriptionAsync(userId)
                                    ?? throw new Exception("You don`t have active subscription");

        if (getActiveSubscription.IsCanceled)
            throw new Exception("You can`t upgrade canceled subscription");

        var newPriceInformation = await GetPriceService().GetAsync(newPriceId);

        if (newPriceInformation.UnitAmountDecimal / Constants.PriceInCents <= getActiveSubscription.Price)
            throw new Exception("You can`t upgrade on a subscription with the same or a lower price");

        var subscription = await GetSubscriptionService().GetAsync(getActiveSubscription.SubscriptionStripeId);

        if (subscription.Items.Data.Count > 1)
            throw new Exception($"You can`t upgrade your subscription, wait until {subscription.CurrentPeriodEnd}");

        await GetSubscriptionService().UpdateAsync(subscription.Id, new SubscriptionUpdateOptions
        {
            ProrationBehavior = "always_invoice",
            ProrationDate = DateTime.UtcNow,
            Items = new List<SubscriptionItemOptions>
            {
                new() { Id = subscription.Items.Data.First().Id, Deleted = true },
                new() { Price = newPriceId }
            }
        });

        await _subscriptionRepository.UpdateSubscriptionStatusAsync(subscription.Id,
            SubscriptionStatusEnum.PendingUpgrade);
    }

    public async Task DowngradeSubscriptionAsync(Guid userId, string newPriceId)
    {
        var getActiveSubscription = await _paymentCustomerService.GetActiveSubscriptionAsync(userId)
                                    ?? throw new Exception("You don`t have active subscription");

        if (getActiveSubscription.IsCanceled)
            throw new Exception("You can`t downgrade canceled subscription");

        var user = await _userRepository.GetUserByIdAsync(userId)
                   ?? throw new ArgumentNullException(nameof(userId));

        var newPriceInformation = await GetPriceService().GetAsync(newPriceId);
        var newPriceDecimal = newPriceInformation.UnitAmountDecimal / Constants.PriceInCents;

        if (newPriceDecimal >= getActiveSubscription.Price)
            throw new Exception("You can`t downgrade on a subscription with the same or a higher price");

        var subscription = await GetSubscriptionService().GetAsync(getActiveSubscription.SubscriptionStripeId);

        if (subscription.Items.Data.Count > 1)
            throw new Exception($"You can`t downgrade your subscription, wait until {subscription.CurrentPeriodEnd}");

        await GetSubscriptionService().UpdateAsync(subscription.Id, new SubscriptionUpdateOptions
        {
            ProrationBehavior = "none",
            Items = new List<SubscriptionItemOptions>
            {
                new() { Id = subscription.Items.Data.First().Id, Deleted = true },
                new() { Price = newPriceId }
            }
        });

        getActiveSubscription.Price = newPriceDecimal;
        getActiveSubscription.IsCanceled = true;

        await _subscriptionRepository.UpdateSubscriptionAsync(getActiveSubscription, user.AccountPlan);

        await _subscriptionRepository.UpdateSubscriptionStatusAsync(subscription.Id,
            SubscriptionStatusEnum.PendingDowngrade);
    }

    private async Task IfUserAbleToUsePaymentAsync(User user, bool isFreeSubscription = false)
    {
        if (string.IsNullOrEmpty(user.StripeCustomerId))
            throw new Exception("You have to create a customer billing record");

        if (isFreeSubscription)
            return;

        var retrievedCustomer = await GetCustomerService().GetAsync(user.StripeCustomerId, new CustomerGetOptions
        {
            Expand = new List<string> { "invoice_settings.default_payment_method" }
        });

        if (retrievedCustomer.InvoiceSettings is null ||
            string.IsNullOrEmpty(retrievedCustomer.InvoiceSettings.DefaultPaymentMethodId))
            throw new Exception("You have to add payment method/card and set it as default");
    }
}