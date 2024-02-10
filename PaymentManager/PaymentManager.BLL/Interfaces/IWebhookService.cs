using Stripe;
using Stripe.Checkout;

namespace PaymentManager.BLL.Interfaces;

/// <summary>
///     Interface defining methods for handling webhook events.
/// </summary>
public interface IWebhookService
{
    /// <summary>
    ///     Creates a customer asynchronously.
    /// </summary>
    /// <param name="customer">The customer to be created</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task CreateCustomerAsync(Customer customer);

    /// <summary>
    ///     Deletes a subscription asynchronously.
    /// </summary>
    /// <param name="subscription">The subscription to be deleted</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task DeleteSubscriptionAsync(Subscription subscription);

    /// <summary>
    ///     Expires a session asynchronously.
    /// </summary>
    /// <param name="session">The session to be expired</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task ExpireSessionAsync(Session session);

    /// <summary>
    ///     Handles order confirmation asynchronously.
    /// </summary>
    /// <param name="session">The session related to the order</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task OrderConfirmationAsync(Session session);

    /// <summary>
    ///     Handles payment failure asynchronously.
    /// </summary>
    /// <param name="invoice">The invoice related to the failed payment</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task InvoiceFailedHandleAsync(Invoice invoice);

    /// <summary>
    ///     Handles successful payment asynchronously.
    /// </summary>
    /// <param name="invoice">The invoice related to the successful payment</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task InvoiceSucceededHandleAsync(Invoice invoice);

    /// <summary>
    ///     Updates a subscription asynchronously.
    /// </summary>
    /// <param name="subscription">The subscription to be updated</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task UpdateSubscriptionAsync(Subscription subscription);
}