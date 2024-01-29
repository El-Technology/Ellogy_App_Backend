using PaymentManager.BLL.Models;
using Stripe.Checkout;

namespace PaymentManager.BLL.Interfaces
{
    /// <summary>
    /// Interface defining methods for handling user payments and order confirmations.
    /// </summary>
    public interface IPaymentSessionService
    {
        Task CancelSubscriptionAsync(Guid userId);

        /// <summary>
        /// Creates a payment session asynchronously for a user with the given parameters.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="streamRequest">Request parameters for payment creation</param>
        /// <returns>Task representing the asynchronous operation, returning the session creation options</returns>
        Task<SessionCreateOptions> CreateOneTimePaymentAsync(Guid userId, CreatePaymentRequest streamRequest);
        Task<SessionCreateOptions> CreateSubscriptionAsync(CreateSubscriptionRequest сreateSubscriptionRequest, Guid userId);

        /// <summary>
        /// Expires a payment session based on the provided session ID asynchronously.
        /// </summary>
        /// <param name="sessionId">Payment session ID</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task ExpireSessionAsync(Session session);

        /// <summary>
        /// Retrieves the balance of a user based on their ID asynchronously.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Task representing the asynchronous operation, returning the user's balance</returns>
        Task<int> GetUserBalanceAsync(Guid userId);

        /// <summary>
        /// Handles order confirmation for a payment session asynchronously based on the provided session ID.
        /// </summary>
        /// <param name="session">Payment session ID</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task OrderConfirmationAsync(Session session);
    }
}
