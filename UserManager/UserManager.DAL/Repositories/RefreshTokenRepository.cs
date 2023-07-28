using Microsoft.EntityFrameworkCore;
using UserManager.DAL.Context;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.DAL.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly UserManagerDbContext _context;
        public RefreshTokenRepository(UserManagerDbContext context)
        {
            _context = context;
        }

        public async Task UpdateTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(Guid userId)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(a => a.UserId.Equals(userId));
        }
    }
}
