using UserManager.DAL.Models;

namespace UserManager.DAL.Interfaces
{
    public interface IRefreshTokenRepository
    {
        public Task AddRefreshTokenAsync(RefreshToken refreshToken);
        public Task<RefreshToken?> GetRefreshTokenAsync(Guid userId);
        Task UpdateTokenAsync(RefreshToken refreshToken);
    }
}
