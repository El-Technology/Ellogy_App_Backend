using Microsoft.IdentityModel.Tokens;
using UserManager.BLL.Dtos.RefreshTokenDtos;
using UserManager.BLL.Exceptions;
using UserManager.BLL.Helpers;
using UserManager.BLL.Interfaces;
using UserManager.Common.Helpers;
using UserManager.Common.Options;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.BLL.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
        }

        public async Task<string> GetRefreshTokenAsync(Guid userId)
        {
            var refreshTokenFromDb = await _refreshTokenRepository.GetRefreshTokenAsync(userId);
            if (refreshTokenFromDb is not null)
            {
                refreshTokenFromDb.ExpireDate = DateTime.UtcNow + JwtOptions.RefreshTokenLifeTime;
                await _refreshTokenRepository.UpdateTokenExpireDateAsync(refreshTokenFromDb);
                return refreshTokenFromDb.Value;
            }

            var refreshToken = CryptoHelper.GenerateToken();
            var refreshTokenModel = new RefreshToken
            {
                Id = Guid.NewGuid(),
                ExpireDate = DateTime.UtcNow + JwtOptions.RefreshTokenLifeTime,
                IsValid = true,
                UserId = userId,
                Value = refreshToken
            };

            await _refreshTokenRepository.AddRefreshTokenAsync(refreshTokenModel);
            return refreshToken;
        }

        public async Task<string> RegenerateJwtAsync(RefreshTokenRequestDto refreshTokenRequestDto)
        {
            ValidateJwt(refreshTokenRequestDto.Jwt);

            var userId = JwtHelper.GetJwtClaimValue(refreshTokenRequestDto.Jwt, JwtOptions.UserIdClaimName)
                ?? throw new InvalidJwtException();

            var refreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(Guid.Parse(userId))
                ?? throw new InvalidRefreshTokenException();

            ValidateRefreshToken(refreshToken, refreshTokenRequestDto);

            var user = await _userRepository.GetUserByIdAsync(Guid.Parse(userId))
                ?? throw new UserNotFoundException(userId);

            var jwt = JwtHelper.GenerateJwt(user);

            return jwt;
        }

        private static void ValidateRefreshToken(RefreshToken refreshToken, RefreshTokenRequestDto refreshTokenRequest)
        {
            var valueValid = refreshToken.Value == refreshTokenRequest.RefreshToken;
            var expireDateValid = refreshToken.ExpireDate >= DateTime.UtcNow;

            if (!(valueValid && expireDateValid && refreshToken.IsValid))
                throw new InvalidRefreshTokenException();
        }

        private static void ValidateJwt(string jwtToken)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateLifetime = false,
                ValidateAudience = false,
                ValidateIssuer = true,
                ValidIssuer = JwtOptions.Issuer,
                IssuerSigningKey = JwtOptions.GetSymmetricSecurityKey()
            };
            JwtHelper.ValidateJwt(jwtToken, validationParameters);
        }
    }
}
