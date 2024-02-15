using PaymentManager.DAL.Enums;
using PaymentManager.DAL.Models;

namespace PaymentManager.DAL.Interfaces;

/// <summary>
///     Interface defining methods for interacting with subscription data.
/// </summary>
public interface ISubscriptionRepository
{
    /// <summary>
    ///     Creates a subscription asynchronously with the specified subscription and account plan.
    /// </summary>
    /// <param name="subscription">Subscription information</param>
    /// <param name="accountPlan">Account plan information</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task CreateSubscriptionAsync(Subscription subscription, AccountPlan accountPlan);

    /// <summary>
    ///     Retrieves the active subscription asynchronously for a given user ID.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task representing the asynchronous operation, returning the active subscription if available; otherwise, null</returns>
    Task<Subscription?> GetActiveSubscriptionAsync(Guid userId);

    /// <summary>
    ///     Updates the subscription asynchronously with the specified subscription and account plan.
    /// </summary>
    /// <param name="subscription">Subscription information</param>
    /// <param name="accountPlan">Account plan information</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task UpdateSubscriptionAsync(Subscription subscription, AccountPlan? accountPlan);

    /// <summary>
    ///     Updates the subscription is canceled field with the specified stripe ID asynchronously.
    /// </summary>
    /// <param name="stripeId">Stripe ID</param>
    /// <param name="isCanceled">Indicates whether the subscription is canceled</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task UpdateSubscriptionIsCanceledAsync(string stripeId, bool isCanceled);

    Task UpdateSubscriptionStatusAsync(string stripeId, SubscriptionStatusEnum status);
}