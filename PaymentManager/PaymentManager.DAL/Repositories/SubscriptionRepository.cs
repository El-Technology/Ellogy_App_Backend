using Microsoft.EntityFrameworkCore;
using PaymentManager.DAL.Context.PaymentContext;
using PaymentManager.DAL.Enums;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;

namespace PaymentManager.DAL.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly IUserRepository _userRepository;
        private readonly PaymentContext _context;
        public SubscriptionRepository(IUserRepository userRepository, PaymentContext context)
        {
            _context = context;
            _userRepository = userRepository;
        }

        /// <inheritdoc cref="ISubscriptionRepository.CreateSubscriptionAsync(Subscription, AccountPlan)"/>
        public async Task CreateSubscriptionAsync(Subscription subscription, AccountPlan accountPlan)
        {
            await _context.Subscriptions.AddAsync(subscription);
            await _context.SaveChangesAsync();

            await _userRepository.UpdateAccountPlanAsync(subscription.UserId, accountPlan);
        }

        /// <inheritdoc cref="ISubscriptionRepository.UpdateSubscriptionAsync(Subscription, AccountPlan)"/>
        public async Task UpdateSubscriptionAsync(Subscription subscription, AccountPlan? accountPlan)
        {
            await _context.Subscriptions
                .Where(a => a.SubscriptionStripeId.Equals(subscription.SubscriptionStripeId))
                .ExecuteUpdateAsync(a => a
                    .SetProperty(a => a.IsActive, a => subscription.IsActive)
                    .SetProperty(a => a.EndDate, a => subscription.EndDate)
                    .SetProperty(a => a.StartDate, a => subscription.StartDate)
                    .SetProperty(a => a.IsCanceled, a => subscription.IsCanceled));

            await _userRepository.UpdateAccountPlanAsync(subscription.UserId, accountPlan);
        }

        /// <inheritdoc cref="ISubscriptionRepository.GetActiveSubscriptionAsync(Guid)"/>
        public async Task<Subscription?> GetActiveSubscriptionAsync(Guid userId)
        {
            return await _context.Subscriptions
                .FirstOrDefaultAsync(a => a.UserId == userId && a.IsActive == true);
        }
    }
}
