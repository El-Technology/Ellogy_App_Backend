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

        public async Task<string> UpdateCustomerPaymentMethodAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException(nameof(userId));

            if (user.StripeCustomerId is null)
                throw new Exception("Customer is not created");

            var sessionService = new SessionService();

            var sessionOptions = new SessionCreateOptions
            {
                SuccessUrl = "http://string.com",
                CancelUrl = "http://string.com",
                Mode = "setup",
                Currency = Constants.ApplicationCurrency,
                Customer = user.StripeCustomerId
            };

            var result = await sessionService.CreateAsync(sessionOptions);
            return result.Url;
        }
    }
}