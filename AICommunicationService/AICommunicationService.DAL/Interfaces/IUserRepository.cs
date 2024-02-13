using AICommunicationService.Common.Dtos;
using AICommunicationService.DAL.Models;

namespace AICommunicationService.DAL.Interfaces;

/// <summary>
///     Interface defining methods for interacting with user data in a repository.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    ///     Retrieves a user by their unique identifier asynchronously.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task representing the asynchronous operation, returning a nullable User object</returns>
    Task<User?> GetUserByIdAsync(Guid userId);

    /// <summary>
    ///     Retrieves the total points usage for a user based on their ID asynchronously.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task representing the asynchronous operation, returning the total points used</returns>
    Task<int> GetUserTotalPointsUsageAsync(Guid userId);

    /// <summary>
    ///     Updates the total points usage for a user based on their ID asynchronously.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="usedTokens">Number of tokens used</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task UpdateUserTotalPointsUsageAsync(Guid userId, int usedTokens);

    /// <summary>
    ///     Retrieves a list of users based on their email prefix asynchronously.
    /// </summary>
    /// <param name="emailPrefix"></param>
    /// <returns></returns>
    Task<List<User>> FindUserByEmailAsync(string emailPrefix);

    /// <summary>
    ///     Retrieves a list of users based on their unique identifiers asynchronously.
    /// </summary>
    /// <param name="userIds">List of user IDs</param>
    /// <param name="paginationRequest">Pagination request object</param>
    /// <returns>Task representing the asynchronous operation, returning a list of users</returns>
    Task<PaginationResponseDto<User>> GetUsersByIds(List<Guid> userIds, PaginationRequestDto paginationRequest);
}