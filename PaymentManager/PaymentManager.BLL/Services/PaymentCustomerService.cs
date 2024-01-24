using PaymentManager.DAL.Interfaces;
using Stripe;

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

            var options = new CustomerCreateOptions
            {
                Email = user.Email,
                Name = $"{user.FirstName} {user.LastName}",
            };

            var service = new CustomerService();
            var customerData = await service.CreateAsync(options);

            await _userRepository.AddStripeCustomerIdAsync(userId, customerData.Id);
        }
    }
}
