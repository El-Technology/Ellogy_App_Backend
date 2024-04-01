using Microsoft.EntityFrameworkCore;
using UserManager.Common.Dtos;
using UserManager.DAL.Context;
using UserManager.DAL.Extensions;
using UserManager.DAL.Models;

namespace UserManager.DAL.Repositories;
public class UserExternalRepository
{
    private readonly UserManagerDbContext _context;
    public UserExternalRepository(UserManagerDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc cref="IUserRepository.UpdateUserTotalPointsUsageAsync(Guid, int)" />
    public async Task UpdateUserTotalPointsUsageAsync(Guid userId, int usedTokens)
    {
        await _context.Users
            .Where(a => a.Id == userId)
            .ExecuteUpdateAsync(x => x.SetProperty(x => x.TotalPointsUsage, x => x.TotalPointsUsage + usedTokens));
    }

    /// <inheritdoc cref="IUserRepository.GetUserTotalPointsUsageAsync(Guid)" />
    public async Task<int> GetUserTotalPointsUsageAsync(Guid userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == userId)
                   ?? throw new Exception($"User with id - {userId} was not found");

        return user.TotalPointsUsage;
    }

    /// <inheritdoc cref="IUserRepository.GetUserByIdAsync(Guid)" />
    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
    }

    /// <inheritdoc cref="IUserRepository.FindUserByEmailAsync(string)" />
    public async Task<List<User>> FindUserByEmailAsync(string emailPrefix)
    {
        return await _context.Users
            .Where(u => u.Email.StartsWith(emailPrefix))
            .Take(5)
            .ToListAsync();
    }

    /// <inheritdoc cref="IUserRepository.GetUsersByIdsWithPaginationAsync" />
    public async Task<PaginationResponseDto<User>> GetUsersByIdsWithPaginationAsync(List<Guid> userIds,
        PaginationRequestDto paginationRequest)
    {
        return await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .GetUsersPaginatedResult(paginationRequest);
    }

    /// <inheritdoc cref="IUserRepository.GetUsersByIdsAsync" />
    public async Task<List<User>> GetUsersByIdsAsync(List<Guid> userIds)
    {
        return await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync();
    }
}
