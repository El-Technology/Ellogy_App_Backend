using PaymentManager.DAL.Interfaces;
using Stripe;
using Stripe.Checkout;

namespace PaymentManager.BLL.Services
{
    public class PaymentSubscriptionService
    {
        private readonly IUserRepository _userRepository;
        public PaymentSubscriptionService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<string> CreateSubscriptionAsync(Guid userId, string priceId)
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

            if (customerData.Subscriptions is not null && customerData.Subscriptions.Any())
                throw new Exception("You have to cancel your existing subscription before having a new one");

            var sessionService = new SessionService();
            var sessionCreateOptions = new SessionCreateOptions()
            {
                SuccessUrl = "http://string.com",
                CancelUrl = "http://string.com",
                Customer = user.StripeCustomerId,
                Mode = "subscription",
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = priceId,
                        Quantity = 1,
                    },
                }
            };

            var creationResult = await sessionService.CreateAsync(sessionCreateOptions);
            return creationResult.Url;
        }

        public async Task CancelSubscriptionAsync(Guid userId, bool cancelNow, string paymentIntent)
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

            if (customerData.Subscriptions is null && !customerData.Subscriptions.Any())
                throw new Exception("You don`t have any subscription");

            var subscriptionService = new SubscriptionService();
            var currentSubscription = await subscriptionService.GetAsync(customerData.Subscriptions.FirstOrDefault()?.Id);

            if (cancelNow)
            {
                await subscriptionService.CancelAsync(currentSubscription.Id);

                var refundService = new RefundService();
                var refundServiceOptions = new RefundCreateOptions
                {
                    PaymentIntent = paymentIntent
                };

                await refundService.CreateAsync(refundServiceOptions);

                return;
            }

            var subscriptionUpdateOptions = new SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = true
            };

            await subscriptionService.UpdateAsync(currentSubscription.Id, subscriptionUpdateOptions);
        }
    }
}
