using UserManager.BLL.Dtos.RefreshTokenDtos;

namespace UserManager.BLL.Interfaces;

public interface IRefreshTokenService
{
    /// <summary>
    ///     Get refresh token for user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task<string> GetRefreshTokenAsync(Guid userId);

    /// <summary>
    ///     Regenerate JWT token
    /// </summary>
    /// <param name="refreshTokenRequestDto"></param>
    /// <returns></returns>
    public Task<string> RegenerateJwtAsync(RefreshTokenRequestDto refreshTokenRequestDto);
}