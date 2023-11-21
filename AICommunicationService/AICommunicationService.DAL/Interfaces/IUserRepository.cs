using AICommunicationService.DAL.Models;

namespace AICommunicationService.DAL.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<int> GetUserTotalPointsUsageAsync(Guid userId);
        Task UpdateUserTotalPointsUsageAsync(Guid userId, int usedTokens);
    }
}
