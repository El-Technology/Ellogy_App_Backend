using PaymentManager.BLL.Models;
using PaymentManager.Common.Constants;
using PaymentManager.DAL.Interfaces;
using Stripe;
using Stripe.Checkout;

namespace PaymentManager.BLL.Services
{
    public class PaymentCustomerService
    {
        private readonly IUserRepository _userRepository;
        public PaymentCustomerService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task CreateCustomerAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException(nameof(userId));

            if (!string.IsNullOrEmpty(user.StripeCustomerId))
                throw new Exception("Customer is already created");

            var options = new CustomerCreateOptions
            {
                Email = user.Email,
                Name = $"{user.FirstName} {user.LastName}",
            };

            var service = new CustomerService();
            var customerData = await service.CreateAsync(options);

            await _userRepository.AddStripeCustomerIdAsync(userId, customerData.Id);
        }

        public async Task UpdateCustomerDataAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException(nameof(userId));

            if (string.IsNullOrEmpty(user.StripeCustomerId))
                throw new Exception("Customer is not created");

            var service = new CustomerService();

            var updateOptions = new CustomerUpdateOptions
            {
                Email = user.Email,
                Name = $"{user.FirstName} {user.LastName}"
            };

            await service.UpdateAsync(user.StripeCustomerId, updateOptions);
        }

        public async Task<SessionCreateOptions> AddCustomerPaymentMethodAsync(Guid userId, CreateSessionRequest createSessionRequest)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException(nameof(userId));

            if (user.StripeCustomerId is null)
                throw new ArgumentNullException("Customer is not created");

            var sessionOptions = new SessionCreateOptions
            {
                SuccessUrl = createSessionRequest.SuccessUrl,
                CancelUrl = createSessionRequest.CancelUrl,
                Mode = Constants.SETUP_MODE,
                Currency = Constants.ApplicationCurrency,
                Customer = user.StripeCustomerId,
                Metadata = new Dictionary<string, string>
                {
                    { MetadataConstants.UserId, user.Id.ToString() },
                    { MetadataConstants.ConnectionId, createSessionRequest.ConnectionId },
                    { MetadataConstants.SignalRMethodName, createSessionRequest.SignalMethodName }
                }
            };

            return sessionOptions;
        }

        public async IAsyncEnumerable<object> RetrieveCustomerPaymentMethodsAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException(nameof(userId));

            if (string.IsNullOrEmpty(user.StripeCustomerId))
                throw new Exception("Customer is not created");

            var paymentService = new PaymentMethodService();

            var allMethods = await paymentService.ListAsync(new PaymentMethodListOptions { Customer = user.StripeCustomerId });
            foreach (var method in allMethods)
            {
                yield return new
                {
                    Id = method.Id,
                    CardBrand = method.Card.Brand,
                    Expires = $"{method.Card.ExpMonth}/{method.Card.ExpYear}",
                    Last4 = method.Card.Last4
                };
            }
        }

        public async Task SetDefaultPaymentMethodAsync(Guid userId, string paymentMethodId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException(nameof(userId));

            var customerService = new CustomerService();
            var updateOptions = new CustomerUpdateOptions
            {
                InvoiceSettings = new()
                {
                    DefaultPaymentMethod = paymentMethodId,
                }
            };

            await customerService.UpdateAsync(user.StripeCustomerId, updateOptions);

            var customer = await customerService.GetAsync(user.StripeCustomerId,
                new CustomerGetOptions
                {
                    Expand = new() { "subscriptions" }
                });

            var customerSubscription = customer.Subscriptions.FirstOrDefault()
                ?? throw new NullReferenceException("Subscription null exception");

            var subscriptionService = new SubscriptionService();

            await subscriptionService.UpdateAsync(customerSubscription!.Id,
                new SubscriptionUpdateOptions
                {
                    DefaultPaymentMethod = paymentMethodId
                });
        }
    }
}