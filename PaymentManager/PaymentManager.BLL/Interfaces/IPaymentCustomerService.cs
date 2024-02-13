using PaymentManager.BLL.Models;
using PaymentManager.Common.Dtos;
using PaymentManager.DAL.Models;
using Stripe.Checkout;

namespace PaymentManager.BLL.Interfaces;

/// <summary>
///     Interface defining methods for managing customer payments and subscriptions.
/// </summary>
public interface IPaymentCustomerService
{
    /// <summary>
    ///     Deletes a customer asynchronously.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task DeleteCustomerAsync(Guid userId);

    /// <summary>
    ///     Adds a customer's payment method asynchronously.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="createSessionRequest">Request parameters for creating the session</param>
    /// <returns>Task representing the asynchronous operation, returning the session creation options</returns>
    Task<SessionCreateOptions> AddCustomerPaymentMethodAsync(Guid userId, CreateSessionRequest createSessionRequest);

    /// <summary>
    ///     Creates a customer asynchronously.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task CreateCustomerAsync(Guid userId);

    /// <summary>
    ///     Detaches a payment method from a customer asynchronously.
    /// </summary>
    /// <param name="paymentMethodId"></param>
    /// <returns></returns>
    Task DetachPaymentMethodAsync(string paymentMethodId);

    /// <summary>
    ///     Retrieves the active subscription of a user asynchronously.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task representing the asynchronous operation, returning the active subscription if available</returns>
    Task<Subscription?> GetActiveSubscriptionAsync(Guid userId);

    /// <summary>
    ///     Retrieves customer payments asynchronously.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="paginationRequestDto"></param>
    /// <returns>An asynchronous enumerable collection of customer payments</returns>
    Task<StripePaginationResponseDto<IEnumerable<PaymentObject>>> GetCustomerPaymentsAsync(Guid userId,
        StripePaginationRequestDto paginationRequestDto);

    /// <summary>
    ///     Retrieves customer payment methods asynchronously.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="paginationRequestDto"></param>
    /// <returns>An asynchronous enumerable collection of customer payment methods</returns>
    Task<StripePaginationResponseDto<IEnumerable<PaymentMethod>>> RetrieveCustomerPaymentMethodsAsync(Guid userId,
        StripePaginationRequestDto paginationRequestDto);

    /// <summary>
    ///     Sets the default payment method for a customer asynchronously.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="paymentMethodId">Payment method ID</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task SetDefaultPaymentMethodAsync(Guid userId, string paymentMethodId);

    /// <summary>
    ///     Updates customer data asynchronously.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task UpdateCustomerDataAsync(Guid userId);

    /// <summary>
    ///     Upgrades a user's subscription preview asynchronously.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="newPriceId">New price ID</param>
    /// <returns>Task representing the asynchronous operation, returning the preview price</returns>
    Task<decimal> UpgradeSubscriptionPreviewAsync(Guid userId, string newPriceId);
}