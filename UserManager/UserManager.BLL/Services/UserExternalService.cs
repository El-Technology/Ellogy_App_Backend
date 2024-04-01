using UserManager.Common.Dtos;
using UserManager.DAL.Enums;
using UserManager.DAL.Models;
using UserManager.DAL.Repositories;

namespace UserManager.BLL.Services;
public class UserExternalService
{
    private readonly UserExternalRepository _userExternalRepository;
    public UserExternalService(UserExternalRepository userExternalRepository)
    {
        _userExternalRepository = userExternalRepository;
    }

    public async Task UpdateUserTotalPointsUsageAsync(Guid userId, int usedTokens)
    {
        await _userExternalRepository.UpdateUserTotalPointsUsageAsync(userId, usedTokens);
        await Task.CompletedTask;
    }

    public async Task<int> GetUserTotalPointsUsageAsync(Guid userId)
    {
        return await _userExternalRepository.GetUserTotalPointsUsageAsync(userId);
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _userExternalRepository.GetUserByIdAsync(userId);
    }

    public async Task<List<User>> FindUserByEmailAsync(string emailPrefix)
    {
        return await _userExternalRepository.FindUserByEmailAsync(emailPrefix);
    }

    public async Task<List<User>> GetUsersByIdsAsync(List<Guid> userIds)
    {
        return await _userExternalRepository.GetUsersByIdsAsync(userIds);
    }

    public async Task<PaginationResponseDto<User>> GetUsersByIdsWithPaginationAsync(List<Guid> userIds,
               PaginationRequestDto paginationRequest)
    {
        return await _userExternalRepository.GetUsersByIdsWithPaginationAsync(userIds, paginationRequest);
    }

    public async Task AddStripeCustomerIdAsync(Guid userId, string customerId)
    {
        await _userExternalRepository.AddStripeCustomerIdAsync(userId, customerId);
        await Task.CompletedTask;
    }

    public async Task RemoveStripeCustomerIdAsync(Guid userId)
    {
        await _userExternalRepository.RemoveStripeCustomerIdAsync(userId);
        await Task.CompletedTask;
    }

    public async Task UpdateTotalPurchasedTokensAsync(Guid userId, int totalPurchasedTokens)
    {
        await _userExternalRepository.UpdateTotalPurchasedTokensAsync(userId, totalPurchasedTokens);
        await Task.CompletedTask;
    }

    public async Task UpdateAccountPlanAsync(Guid userId, AccountPlan accountPlan)
    {
        await _userExternalRepository.UpdateAccountPlanAsync(userId, accountPlan);
        await Task.CompletedTask;
    }
}
