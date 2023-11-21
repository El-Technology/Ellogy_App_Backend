using AICommunicationService.DAL.Models;

namespace AICommunicationService.DAL.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<int> GetUserTotalTokensUsageAsync(Guid userId);
        Task UpdateUserTotalTokensUsageAsync(Guid userId, int usedTokens);
    }
}
