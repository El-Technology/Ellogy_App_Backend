using UserManager.DAL.Models;

namespace UserManager.DAL.Interfaces;

public interface IRefreshTokenRepository
{
    /// <summary>
    ///     Add refresh token to database
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    public Task AddRefreshTokenAsync(RefreshToken refreshToken);

    /// <summary>
    ///     Get refresh token for user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task<RefreshToken?> GetRefreshTokenAsync(Guid userId);

    /// <summary>
    ///     Update refresh token
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    Task UpdateTokenAsync(RefreshToken refreshToken);
}