using PaymentManager.BLL.Models;
using Stripe;
using Stripe.Checkout;

namespace PaymentManager.BLL.Interfaces;

/// <summary>
///     Interface defining methods for handling user payments and order confirmations.
/// </summary>
public interface IPaymentSessionService
{
    /// <summary>
    ///     Cancels a user's subscription asynchronously.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task CancelSubscriptionAsync(Guid userId);

    /// <summary>
    ///     Creates free subscription asynchronously for a user.
    /// </summary>
    /// <param name="signalRModel"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<SubscriptionCreateOptions> CreateFreeSubscriptionAsync(SignalRModel signalRModel, Guid userId);

    /// <summary>
    ///     Creates a one-time payment session asynchronously for a user with the given parameters.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="streamRequest">Request parameters for payment creation</param>
    /// <returns>Task representing the asynchronous operation, returning the session creation options</returns>
    Task<SessionCreateOptions> CreateOneTimePaymentAsync(Guid userId, CreatePaymentRequest streamRequest);

    /// <summary>
    ///     Retrieves the balance of a user based on their ID asynchronously.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task representing the asynchronous operation, returning the user's balance</returns>
    Task<int> GetUserBalanceAsync(Guid userId);

    /// <summary>
    ///     Upgrades a user's subscription asynchronously.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="newPriceId">New price ID</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task UpgradeSubscriptionAsync(Guid userId, string newPriceId);
}