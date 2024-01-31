using Microsoft.Extensions.Logging;
using PaymentManager.BLL.Helpers;
using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Models;
using PaymentManager.Common.Constants;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;
using Stripe;
using Stripe.Checkout;

namespace PaymentManager.BLL.Services
{
    public class PaymentSessionService : IPaymentSessionService
    {
        private readonly ILogger<PaymentSessionService> _logger;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ProductCatalogService _productCatalogService;

        private const int AMMOUNT_OF_ITEMS = 1;

        public PaymentSessionService(IPaymentRepository paymentRepository,
            IUserRepository userRepository,
            ILogger<PaymentSessionService> logger,
            ProductCatalogService productCatalogService)
        {
            _productCatalogService = productCatalogService;
            _logger = logger;
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
        }

        private async Task IfUserAbleToUsePaymentAsync(User user)
        {
            if (string.IsNullOrEmpty(user.StripeCustomerId))
                throw new ArgumentNullException("You have to create a customer billing record");

            var customerService = new CustomerService();
            var retrievedCustomer = await customerService.GetAsync(user.StripeCustomerId, new()
            {
                Expand = new() { "invoice_settings.default_payment_method" }
            });

            if (string.IsNullOrEmpty(retrievedCustomer.InvoiceSettings.DefaultPaymentMethodId))
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
                            UnitAmountDecimal = (decimal)(streamRequest.AmountOfPoints * PointPriceConstant.OneTokenPrice * PointPriceConstant.PriceInCents),
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

        public async Task<SessionCreateOptions> CreateSubscriptionAsync(CreateSubscriptionRequest createSubscriptionRequest, Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException(nameof(userId));

            await IfUserAbleToUsePaymentAsync(user);

            var customerService = new CustomerService();
            var customerData = await customerService.GetAsync(user.StripeCustomerId,
                                new CustomerGetOptions
                                {
                                    Expand = new() { "subscriptions" }
                                })
                ?? throw new Exception("We can`t find you payment profile");

            if (customerData.Subscriptions is not null && customerData.Subscriptions.Any())
                throw new Exception("You have to cancel your existing subscription before having a new one");

            var product = await _productCatalogService.GetProductAsync(createSubscriptionRequest.ProductId);

            var sessionCreateOptions = new SessionCreateOptions()
            {
                SuccessUrl = createSubscriptionRequest.SuccessUrl,
                CancelUrl = createSubscriptionRequest.CancelUrl,
                Customer = user.StripeCustomerId,
                Mode = Constants.SUBSCRIPTION_MODE,
                LineItems = new List<SessionLineItemOptions>
                {
                    new ()
                    {
                        Price = product.PriceId,
                        Quantity = AMMOUNT_OF_ITEMS
                    },
                },
                Metadata = new Dictionary<string, string>
                {
                    { MetadataConstants.ProductName,  product.Name},
                    { MetadataConstants.AmountOfPoint, "0" },
                    { MetadataConstants.UserId, user.Id.ToString() },
                    { MetadataConstants.ConnectionId, createSubscriptionRequest.ConnectionId },
                    { MetadataConstants.SignalRMethodName, createSubscriptionRequest.SignalMethodName }
                },
                SubscriptionData = new()
                {
                    Metadata = new() {
                        { MetadataConstants.AccountPlan, SubscriptionHelper.GetSubscriptionCode(product.Name).ToString() },
                        { MetadataConstants.UserId, user.Id.ToString() },
                        { MetadataConstants.ProductName,  product.Name}}
                }
            };

            return sessionCreateOptions;
        }

        public async Task CancelSubscriptionAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException(nameof(userId));

            var customerService = new CustomerService();
            var customerData = await customerService.GetAsync(user.StripeCustomerId,
                                new CustomerGetOptions
                                {
                                    Expand = new() { "subscriptions" }
                                })
                ?? throw new Exception("We can`t find you payment profile");

            if (customerData.Subscriptions?.Any() != true)
                throw new Exception("You don`t have any subscription");

            var subscriptionService = new SubscriptionService();

            var subscriptionUpdateOptions = new SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = true
            };

            await subscriptionService.UpdateAsync(customerData.Subscriptions.FirstOrDefault()?.Id, subscriptionUpdateOptions);
        }
    }
}
