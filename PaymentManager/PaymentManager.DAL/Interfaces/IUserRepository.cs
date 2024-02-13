using PaymentManager.DAL.Enums;
using PaymentManager.DAL.Models;

namespace PaymentManager.DAL.Interfaces;

/// <summary>
///     Interface defining methods for managing user data in a repository.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    ///     Adds a Stripe customer ID asynchronously for a user.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="customerId">Stripe customer ID</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task AddStripeCustomerIdAsync(Guid userId, string customerId);

    /// <summary>
    ///     Retrieves a list of all users asynchronously.
    /// </summary>
    /// <returns>Task representing the asynchronous operation, returning a list of User objects</returns>
    Task<List<User>> GetAllUsersAsync();

    /// <summary>
    ///     Retrieves a user by their unique identifier asynchronously.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task representing the asynchronous operation, returning a nullable User object</returns>
    Task<User?> GetUserByIdAsync(Guid userId);

    /// <summary>
    ///     Updates the account plan for a user asynchronously.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="accountPlan">Account plan to update</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task UpdateAccountPlanAsync(Guid userId, AccountPlan? accountPlan);

    /// <summary>
    ///     Updates the total number of purchased tokens for a user asynchronously.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="purchasedTokens">Number of purchased tokens to update</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task UpdateTotalPurchasedTokensAsync(Guid userId, int purchasedTokens);

    Task RemoveStripeCustomerIdAsync(Guid userId);
}