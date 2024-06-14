using PaymentManager.BLL.Hubs;
using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Interfaces.IHttpServices;
using PaymentManager.BLL.Models;
using PaymentManager.Common.Constants;
using PaymentManager.DAL.Enums;
using PaymentManager.DAL.Interfaces;
using Stripe;
using Stripe.Checkout;

namespace PaymentManager.BLL.Services;

/// <summary>
///     This class contains methods for payment session related operations
/// </summary>
public class PaymentSessionService : StripeBaseService, IPaymentSessionService
{
    private const int AMOUNT_OF_ITEMS = 1;
    private readonly IPaymentCustomerService _paymentCustomerService;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IProductCatalogService _productCatalogService;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserExternalHttpService _userExternalHttpService;

    public PaymentSessionService(
        IPaymentRepository paymentRepository,
        IProductCatalogService productCatalogService,
        IPaymentCustomerService paymentCustomerService,
        ISubscriptionRepository subscriptionRepository,
        IUserExternalHttpService userExternalHttpService)
    {
        _userExternalHttpService = userExternalHttpService;
        _paymentCustomerService = paymentCustomerService;
        _productCatalogService = productCatalogService;
        _paymentRepository = paymentRepository;
        _subscriptionRepository = subscriptionRepository;
    }

    /// <inheritdoc cref="IPaymentSessionService.CreateOneTimePaymentAsync(Guid, CreatePaymentRequest)" />
    public async Task<SessionCreateOptions> CreateOneTimePaymentAsync(Guid userId, CreatePaymentRequest streamRequest)
    {
        if (!PaymentHub.CheckIfConnectionIdExist(streamRequest.ConnectionId))
            throw new Exception($"Connection id - {streamRequest.ConnectionId} does not exist");

        var user = await _userExternalHttpService.GetUserByIdAsync(userId)
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
                            Name = $"{streamRequest.AmountOfTickets * Constants.OneTicketInCredits} - credits"
                        },
                        UnitAmountDecimal = streamRequest.AmountOfTickets * Constants.OneTicketPrice * Constants.PriceInCents
                    },
                    Quantity = AMOUNT_OF_ITEMS
                }
            },
            PaymentIntentData = new SessionPaymentIntentDataOptions
            { Description = $"{streamRequest.AmountOfTickets * Constants.OneTicketInCredits} - credits" },
            Mode = Constants.PaymentMode,
            Customer = string.IsNullOrEmpty(user.StripeCustomerId) ? null : user.StripeCustomerId,
            CustomerEmail = string.IsNullOrEmpty(user.StripeCustomerId) ? user.Email : null,
            Metadata = new Dictionary<string, string>
            {
                { MetadataConstants.ProductName, $"{streamRequest.AmountOfTickets * Constants.OneTicketInCredits} - credits" },
                { MetadataConstants.AmountOfPoint, (streamRequest.AmountOfTickets * Constants.OneTicketInCredits).ToString() },
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
                         ?? await _paymentRepository.CreateUserWalletAsync(userId);

        return userWallet.Balance;
    }

    /// <inheritdoc cref="IPaymentSessionService.CreateFreeSubscriptionAsync(SignalRModel, Guid)" />
    public async Task<SubscriptionCreateOptions> CreateFreeSubscriptionAsync(SignalRModel signalRModel, Guid userId)
    {
        if (!PaymentHub.CheckIfConnectionIdExist(signalRModel.ConnectionId))
            throw new Exception($"Connection id - {signalRModel.ConnectionId} does not exist");

        var user = await _userExternalHttpService.GetUserByIdAsync(userId)
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
        var user = await _userExternalHttpService.GetUserByIdAsync(userId)
                   ?? throw new ArgumentNullException(nameof(userId));

        ArgumentNullException.ThrowIfNull(user.StripeCustomerId, "You have to create a customer billing record");

        var customerData = await GetCustomerService().GetAsync(user.StripeCustomerId, new CustomerGetOptions
        {
            Expand = new List<string> { "subscriptions" }
        }) ?? throw new Exception("We can`t find you payment profile");

        if (customerData.Subscriptions?.Any() != true)
            throw new Exception("You don`t have any subscription");

        var getProductId = customerData.Subscriptions.First().Items.Data.FirstOrDefault()?.Plan.ProductId
                           ?? throw new Exception("Taking productId error");

        var productModel = await _productCatalogService.GetProductAsync(getProductId);

        if (productModel.Name.Contains(AccountPlan.Free.ToString()))
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

        var user = await _userExternalHttpService.GetUserByIdAsync(userId)
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
        await _userExternalHttpService.UpdateAccountPlanAsync(getActiveSubscription.UserId, user.AccountPlan);

        await _subscriptionRepository.UpdateSubscriptionStatusAsync(subscription.Id,
            SubscriptionStatusEnum.PendingDowngrade);
    }

    private async Task IfUserAbleToUsePaymentAsync(UserDto user, bool isFreeSubscription = false)
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