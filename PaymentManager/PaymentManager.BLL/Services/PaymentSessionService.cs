using Microsoft.Extensions.Logging;
using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Models;
using PaymentManager.Common.Constants;
using PaymentManager.DAL.Enums;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;
using Stripe;
using Stripe.Checkout;

namespace PaymentManager.BLL.Services
{
    public class PaymentSessionService : StripeBaseService, IPaymentSessionService
    {
        private readonly ILogger<PaymentSessionService> _logger;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductCatalogService _productCatalogService;
        private readonly IPaymentCustomerService _paymentCustomerService;

        private const int AMMOUNT_OF_ITEMS = 1;

        public PaymentSessionService(IPaymentRepository paymentRepository,
            IUserRepository userRepository,
            ILogger<PaymentSessionService> logger,
            IProductCatalogService productCatalogService,
            IPaymentCustomerService paymentCustomerService)
        {
            _paymentCustomerService = paymentCustomerService;
            _productCatalogService = productCatalogService;
            _logger = logger;
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
        }

        private async Task IfUserAbleToUsePaymentAsync(User user, bool isFreeSubscription = false)
        {
            if (string.IsNullOrEmpty(user.StripeCustomerId))
                throw new ArgumentNullException("You have to create a customer billing record");

            if (isFreeSubscription)
                return;

            var retrievedCustomer = await GetCustomerService().GetAsync(user.StripeCustomerId, new()
            {
                Expand = new() { "invoice_settings.default_payment_method" }
            });

            if (retrievedCustomer.InvoiceSettings is null || string.IsNullOrEmpty(retrievedCustomer.InvoiceSettings.DefaultPaymentMethodId))
                throw new ArgumentNullException("You have to add payment method/card and set it as default");
        }

        /// <inheritdoc cref="IPaymentSessionService.CreateOneTimePaymentAsync(Guid, CreatePaymentRequest)"/>
        public async Task<SessionCreateOptions> CreateOneTimePaymentAsync(Guid userId, CreatePaymentRequest streamRequest)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException($"User with id - {userId} was not found");

            await IfUserAbleToUsePaymentAsync(user);

            var sessionCreateOptions = new SessionCreateOptions()
            {
                SuccessUrl = streamRequest.SuccessUrl,
                CancelUrl = streamRequest.CancelUrl,
                LineItems = new List<SessionLineItemOptions>()
                {
                    new()
                    {
                        PriceData = new()
                        {
                            Currency = Constants.ApplicationCurrency,
                            ProductData = new()
                            {
                                Name = $"{streamRequest.AmountOfPoints} - points"
                            },
                            UnitAmountDecimal = (streamRequest.AmountOfPoints * Constants.OneTokenPrice * Constants.PriceInCents),
                        },
                        Quantity = AMMOUNT_OF_ITEMS
                    }
                },
                Mode = Constants.PAYMENT_MODE,
                Customer = string.IsNullOrEmpty(user.StripeCustomerId) ? null : user.StripeCustomerId,
                CustomerEmail = string.IsNullOrEmpty(user.StripeCustomerId) ? user.Email : null,
                Metadata = new Dictionary<string, string>
                {
                    { MetadataConstants.ProductName,  $"{streamRequest.AmountOfPoints} - points"},
                    { MetadataConstants.AmountOfPoint, streamRequest.AmountOfPoints.ToString() },
                    { MetadataConstants.UserId, user.Id.ToString() },
                    { MetadataConstants.ConnectionId, streamRequest.ConnectionId },
                    { MetadataConstants.SignalRMethodName, streamRequest.SignalMethodName }
                }
            };

            return sessionCreateOptions;
        }

        /// <inheritdoc cref="IPaymentSessionService.GetUserBalanceAsync(Guid)"/>
        public async Task<int> GetUserBalanceAsync(Guid userId)
        {
            var userWallet = await _paymentRepository.GetUserWalletAsync(userId)
                ?? throw new Exception($"We can`t find wallet by userId => {userId}");

            return userWallet.Balance;
        }

        public async Task<SubscriptionCreateOptions> CreateFreeSubscriptionAsync(SignalRModel signalRModel, Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException(nameof(userId));

            if (user.AccountPlan is not null)
                throw new Exception("You already have subscription");

            var product = await _productCatalogService.GetProductByNameAsync(AccountPlan.Free.ToString());

            await IfUserAbleToUsePaymentAsync(user, true);

            return new SubscriptionCreateOptions()
            {
                Customer = user.StripeCustomerId,
                Items = new() { new() { Price = product.PriceId } },
                Metadata = new()
                {
                        { MetadataConstants.AccountPlan, AccountPlan.Free.ToString() },
                        { MetadataConstants.UserId, user.Id.ToString() },
                        { MetadataConstants.ProductName,  product.Name},
                        { MetadataConstants.ConnectionId, signalRModel.ConnectionId },
                        { MetadataConstants.SignalRMethodName, signalRModel.SignalMethodName }
                }
            };
        }

        /// <inheritdoc cref="IPaymentSessionService.CancelSubscriptionAsync(Guid)"/>
        public async Task CancelSubscriptionAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException(nameof(userId));

            var customerData = await GetCustomerService().GetAsync(user.StripeCustomerId, new()
            {
                Expand = new() { "subscriptions" }
            }) ?? throw new Exception("We can`t find you payment profile");

            if (customerData.Subscriptions?.Any() != true)
                throw new Exception("You don`t have any subscription");

            var getProductId = customerData.Subscriptions.First().Items.Data.FirstOrDefault()?.Plan.ProductId
                ?? throw new Exception("Taking productId error");

            var productModel = await _productCatalogService.GetProductAsync(getProductId);
            var productName = productModel.Name.Substring(0, productModel.Name.IndexOf("/"));

            if (productName.Equals(AccountPlan.Free.ToString()))
                throw new Exception("You can`t cancel Free subscription");

            await GetSubscriptionService().UpdateAsync(customerData.Subscriptions.FirstOrDefault()?.Id, new()
            {
                CancelAtPeriodEnd = true
            });
        }

        public async Task UpgradeSubscriptionAsync(Guid userId, string newPriceId)
        {
            var getActiveSubscription = await _paymentCustomerService.GetActiveSubscriptionAsync(userId)
                ?? throw new Exception("You don`t have active subscription");

            var newPriceInformation = await GetPriceService().GetAsync(newPriceId);

            if (newPriceInformation.UnitAmountDecimal / Constants.PriceInCents <= getActiveSubscription.Price)
                throw new Exception("You can`t upgrade on a subscription with the same or a lower price");

            var subscription = await GetSubscriptionService().GetAsync(getActiveSubscription.SubscriptionStripeId);

            if (subscription.Items.Data.Count > 1)
                throw new Exception($"You can`t upgrade your subscription, wait until {subscription.CurrentPeriodEnd}");

            await GetSubscriptionService().UpdateAsync(subscription.Id, new()
            {
                ProrationBehavior = "always_invoice",
                ProrationDate = DateTime.UtcNow,
                Items = new()
                {
                    new(){ Id = subscription.Items.Data.First().Id, Deleted = true },
                    new(){ Price = newPriceId }
                },
            });
        }
    }
}
