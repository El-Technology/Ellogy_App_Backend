using AICommunicationService.DAL.Context.AiCommunication;
using AICommunicationService.DAL.Interfaces;
using AICommunicationService.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace AICommunicationService.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AICommunicationContext _context;
        public UserRepository(AICommunicationContext context)
        {
            _context = context;
        }

        public async Task UpdateUserTotalPointsUsageAsync(Guid userId, int usedTokens)
        {
            await _context.Users
                .Where(a => a.Id == userId)
                .ExecuteUpdateAsync(x => x.SetProperty(x => x.TotalPointsUsage, x => x.TotalPointsUsage + usedTokens));
        }

        public async Task<int> GetUserTotalPointsUsageAsync(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == userId)
                ?? throw new Exception($"User with id - {userId} was not found");

            return user.TotalPointsUsage;
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}
