using Microsoft.EntityFrameworkCore;
using PaymentManager.DAL.Context.PaymentContext;
using PaymentManager.DAL.Enums;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;

namespace PaymentManager.DAL.Repositories;

/// <summary>
///     This class contains methods for working with subscriptions
/// </summary>
public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly PaymentContext _context;
    private readonly IUserRepository _userRepository;

    public SubscriptionRepository(IUserRepository userRepository, PaymentContext context)
    {
        _context = context;
        _userRepository = userRepository;
    }

    /// <inheritdoc cref="ISubscriptionRepository.CreateSubscriptionAsync(Subscription, AccountPlan)" />
    public async Task CreateSubscriptionAsync(Subscription subscription, AccountPlan accountPlan)
    {
        await _context.Subscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();

        await _userRepository.UpdateAccountPlanAsync(subscription.UserId, accountPlan);
    }

    /// <inheritdoc cref="ISubscriptionRepository.UpdateSubscriptionAsync(Subscription, AccountPlan)" />
    public async Task UpdateSubscriptionAsync(Subscription subscription, AccountPlan? accountPlan)
    {
        var updatedRows = await _context.Subscriptions
            .Where(a => a.SubscriptionStripeId.Equals(subscription.SubscriptionStripeId))
            .ExecuteUpdateAsync(a => a
                .SetProperty(a => a.Name, a => subscription.Name)
                .SetProperty(a => a.Price, a => subscription.Price)
                .SetProperty(a => a.IsActive, a => subscription.IsActive)
                .SetProperty(a => a.EndDate, a => subscription.EndDate)
                .SetProperty(a => a.StartDate, a => subscription.StartDate)
                .SetProperty(a => a.IsCanceled, a => subscription.IsCanceled));

        if (updatedRows == 0)
            throw new Exception($"{subscription.SubscriptionStripeId} was not found");

        await _userRepository.UpdateAccountPlanAsync(subscription.UserId, accountPlan);
    }

    /// <inheritdoc cref="ISubscriptionRepository.UpdateSubscriptionIsCanceledAsync(string, bool)" />
    public async Task UpdateSubscriptionIsCanceledAsync(string stripeId, bool isCanceled)
    {
        await _context.Subscriptions
            .Where(a => a.SubscriptionStripeId.Equals(stripeId))
            .ExecuteUpdateAsync(a => a
                .SetProperty(a => a.IsCanceled, a => isCanceled));
    }

    /// <inheritdoc cref="ISubscriptionRepository.GetActiveSubscriptionAsync(Guid)" />
    public async Task<Subscription?> GetActiveSubscriptionAsync(Guid userId)
    {
        return await _context.Subscriptions
            .FirstOrDefaultAsync(a => a.UserId == userId && a.IsActive == true);
    }
}