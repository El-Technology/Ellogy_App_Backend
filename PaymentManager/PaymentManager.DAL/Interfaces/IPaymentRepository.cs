using PaymentManager.DAL.Models;

namespace PaymentManager.DAL.Interfaces
{
    /// <summary>
    /// Interface defining methods for managing payment-related data in a repository.
    /// </summary>
    public interface IPaymentRepository
    {
        /// <summary>
        /// Creates a payment record asynchronously based on the provided payment details.
        /// </summary>
        /// <param name="payment">Payment object</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task CreatePaymentAsync(Payment payment);

        /// <summary>
        /// Creates a wallet for a user asynchronously based on their ID.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Task representing the asynchronous operation, returning the created user wallet</returns>
        Task<Wallet> CreateUserWalletAsync(Guid userId);

        /// <summary>
        /// Retrieves a payment record based on the provided session ID asynchronously.
        /// </summary>
        /// <param name="sessionId">Payment session ID</param>
        /// <returns>Task representing the asynchronous operation, returning a nullable Payment object</returns>
        Task<Payment?> GetPaymentAsync(string sessionId);

        /// <summary>
        /// Retrieves a payment record based on the provided payment ID asynchronously.
        /// </summary>
        /// <param name="paymentId">Payment ID</param>
        /// <returns>Task representing the asynchronous operation, returning a nullable Payment object</returns>
        Task<Payment?> GetPaymentByIdAsync(string paymentId);

        /// <summary>
        /// Retrieves payment information asynchronously based on the invoice ID.
        /// </summary>
        /// <param name="invoiceId">The unique identifier of the invoice</param>
        /// <returns>Task representing the asynchronous operation, returning the payment if found; otherwise, null</returns>
        Task<Payment?> GetPaymentByInvoiceIdAsync(string invoiceId);
        Task<Payment?> GetPaymentByInvoiceOrPaymentIdAsync(string paymentId, string invoiceId);

        /// <summary>
        /// Retrieves the wallet of a user based on their ID asynchronously.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Task representing the asynchronous operation, returning a nullable Wallet object</returns>
        Task<Wallet?> GetUserWalletAsync(Guid userId);

        /// <summary>
        /// Updates the balance of a user's wallet based on their ID and the amount of points.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="amountOfPoints">Amount of points to update the balance</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task UpdateBalanceAsync(Guid userId, int amountOfPoints);

        /// <summary>
        /// Updates a payment record asynchronously based on the provided payment details.
        /// </summary>
        /// <param name="payment">Payment object</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task UpdatePaymentAsync(Payment payment);
    }
}
