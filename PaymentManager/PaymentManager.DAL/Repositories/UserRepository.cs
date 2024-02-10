using Microsoft.EntityFrameworkCore;
using PaymentManager.DAL.Context.UserContext;
using PaymentManager.DAL.Enums;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;

namespace PaymentManager.DAL.Repositories;

/// <summary>
///     This class contains methods for interacting with the user table in the database
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly UserContext _userContext;

    public UserRepository(UserContext userContext)
    {
        _userContext = userContext;
    }

    /// <inheritdoc cref="IUserRepository.GetUserByIdAsync(Guid)" />
    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _userContext.Users.FindAsync(userId);
    }

    /// <inheritdoc cref="IUserRepository.GetAllUsersAsync" />
    public async Task<List<User>> GetAllUsersAsync()
    {
        var users = await _userContext.Users.ToListAsync();
        return users;
    }

    /// <inheritdoc cref="IUserRepository.UpdateTotalPurchasedTokensAsync(Guid, int)" />
    public async Task UpdateTotalPurchasedTokensAsync(Guid userId, int purchasedTokens)
    {
        await _userContext.Users
            .Where(a => a.Id == userId)
            .ExecuteUpdateAsync(a =>
                a.SetProperty(a => a.TotalPurchasedPoints, a => a.TotalPurchasedPoints + purchasedTokens));
    }

    /// <inheritdoc cref="IUserRepository.AddStripeCustomerIdAsync(Guid, string)" />
    public async Task AddStripeCustomerIdAsync(Guid userId, string customerId)
    {
        await _userContext.Users
            .Where(a => a.Id == userId)
            .ExecuteUpdateAsync(a => a.SetProperty(a => a.StripeCustomerId, a => customerId));
    }

    /// <inheritdoc cref="IUserRepository.UpdateAccountPlanAsync(Guid, AccountPlan?)" />
    public async Task UpdateAccountPlanAsync(Guid userId, AccountPlan? accountPlan)
    {
        await _userContext.Users.Where(a => a.Id == userId)
            .ExecuteUpdateAsync(a => a.SetProperty(a => a.AccountPlan, a => accountPlan));
    }
}