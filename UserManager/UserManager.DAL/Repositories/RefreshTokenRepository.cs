using Microsoft.EntityFrameworkCore;
using UserManager.DAL.Context.UserContext;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.DAL.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly UserManagerDbContext _context;

    public RefreshTokenRepository(UserManagerDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc cref="IRefreshTokenRepository.UpdateTokenAsync" />
    public async Task UpdateTokenAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="IRefreshTokenRepository.AddRefreshTokenAsync" />
    public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="IRefreshTokenRepository.GetRefreshTokenAsync" />
    public async Task<RefreshToken?> GetRefreshTokenAsync(Guid userId)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(a => a.UserId == userId);
    }
}