using AutoFixture;
using Moq;
using UserManager.BLL.Dtos.RefreshTokenDtos;
using UserManager.BLL.Exceptions;
using UserManager.BLL.Helpers;
using UserManager.BLL.Services;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.Tests.ServiceTests;

[TestFixture]
public class RefreshTokenServiceTest
{
    private Mock<IUserRepository> _userRepository;
    private Mock<IRefreshTokenRepository> _refreshTokenRepository;
    private RefreshTokenService _refreshTokenService;
    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _userRepository = new Mock<IUserRepository>();
        _refreshTokenRepository = new Mock<IRefreshTokenRepository>();
        _refreshTokenService = new RefreshTokenService(_refreshTokenRepository.Object, _userRepository.Object);
    }

    [Test]
    public async Task GetRefreshTokenAsync_ShouldReturnRefreshToken_WhenRefreshTokenExists()
    {
        // Arrange
        var userId = _fixture.Create<Guid>();
        var refreshToken = _fixture.Create<string>();
        var refreshTokenFromDb = _fixture.Create<RefreshToken>();
        refreshTokenFromDb.Value = refreshToken;
        _refreshTokenRepository.Setup(x => x.GetRefreshTokenAsync(userId)).ReturnsAsync(refreshTokenFromDb);

        // Act
        var result = await _refreshTokenService.GetRefreshTokenAsync(userId);

        // Assert
        Assert.That(result, Is.EqualTo(refreshToken));
    }

    [Test]
    public async Task GetRefreshTokenAsync_ShouldGenerateNewRefreshToken_WhenRefreshTokenDoesNotExist()
    {
        // Arrange
        var userId = _fixture.Create<Guid>();
        var refreshToken = _fixture.Create<string>();
        _refreshTokenRepository.Setup(x => x.GetRefreshTokenAsync(userId)).ReturnsAsync(null as RefreshToken);
        _refreshTokenRepository.Setup(x => x.AddRefreshTokenAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);
        _userRepository.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(_fixture.Create<User>());

        // Act
        var result = await _refreshTokenService.GetRefreshTokenAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void RegenerateJwtAsync_ShouldThrowException_WhenJwtIsInvalid()
    {
        // Arrange
        var refreshTokenRequestDto = _fixture.Create<RefreshTokenRequestDto>();
        refreshTokenRequestDto.Jwt = null;

        // Act and Assert
        Assert.ThrowsAsync<InvalidJwtException>(async () =>
            await _refreshTokenService.RegenerateJwtAsync(refreshTokenRequestDto));
    }

    [Test]
    public void RegenerateJwtAsync_ShouldThrowException_WhenRefreshTokenIsInvalid()
    {
        // Arrange
        var refreshTokenRequestDto = _fixture.Create<RefreshTokenRequestDto>();
        var user = _fixture.Create<User>();
        refreshTokenRequestDto.Jwt = JwtHelper.GenerateJwt(user);
        refreshTokenRequestDto.RefreshToken = _fixture.Create<string>();
        _refreshTokenRepository.Setup(x => x.GetRefreshTokenAsync(It.IsAny<Guid>())).ReturnsAsync(null as RefreshToken);

        // Act and Assert
        Assert.ThrowsAsync<InvalidRefreshTokenException>(async () =>
                   await _refreshTokenService.RegenerateJwtAsync(refreshTokenRequestDto));
    }

    [Test]
    public void RegenerateJwtAsync_ShouldGenerateNewJWT_WhenDataIsValid()
    {
        // Arrange
        var refreshTokenRequestDto = _fixture.Create<RefreshTokenRequestDto>();
        var user = _fixture.Create<User>();
        var refreshToken = _fixture.Create<RefreshToken>();
        refreshToken.Value = refreshTokenRequestDto.RefreshToken;
        refreshToken.ExpireDate = DateTime.UtcNow.AddHours(1);
        refreshToken.IsValid = true;
        refreshToken.UserId = user.Id;
        refreshTokenRequestDto.Jwt = JwtHelper.GenerateJwt(user);
        _refreshTokenRepository.Setup(x => x.GetRefreshTokenAsync(It.IsAny<Guid>())).ReturnsAsync(refreshToken);
        _userRepository.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);

        // Act
        var result = _refreshTokenService.RegenerateJwtAsync(refreshTokenRequestDto);

        // Assert
        Assert.That(result, Is.Not.Null);
    }
}
