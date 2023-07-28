using UserManager.BLL.Dtos.RefreshTokenDtos;

namespace UserManager.BLL.Interfaces
{
    public interface IRefreshTokenService
    {
        public Task<string> GetRefreshTokenAsync(Guid userId);
        public Task<string> RegenerateJwtAsync(RefreshTokenRequestDto refreshTokenRequestDto);
    }
}
