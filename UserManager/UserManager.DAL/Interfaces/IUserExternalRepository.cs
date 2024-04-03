using UserManager.Common.Dtos;
using UserManager.DAL.Enums;
using UserManager.DAL.Models;

namespace UserManager.DAL.Interfaces;
public interface IUserExternalRepository
{
    Task AddStripeCustomerIdAsync(Guid userId, string customerId);
    Task<List<User>> FindUserByEmailAsync(string emailPrefix);
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<List<User>> GetUsersByIdsAsync(List<Guid> userIds);
    Task<PaginationResponseDto<User>> GetUsersByIdsWithPaginationAsync(List<Guid> userIds, PaginationRequestDto paginationRequest);
    Task<int> GetUserTotalPointsUsageAsync(Guid userId);
    Task RemoveStripeCustomerIdAsync(Guid userId);
    Task UpdateAccountPlanAsync(Guid userId, AccountPlan? accountPlan);
    Task UpdateTotalPurchasedTokensAsync(Guid userId, int purchasedTokens);
    Task UpdateUserTotalPointsUsageAsync(Guid userId, int usedTokens);
}
