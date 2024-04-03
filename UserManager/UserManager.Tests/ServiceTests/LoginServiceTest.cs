using AutoFixture;
using AutoMapper;
using Moq;
using UserManager.BLL.Dtos.LoginDtos;
using UserManager.BLL.Exceptions;
using UserManager.BLL.Interfaces;
using UserManager.BLL.Mapping;
using UserManager.BLL.Services;
using UserManager.DAL.Enums;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.Tests.ServiceTests;

[TestFixture]
public class LoginServiceTest
{
    private IMapper _mapper;
    private Fixture _fixture;

    private Mock<IRefreshTokenService> _refreshTokenService;
    private Mock<IUserRepository> _userRepository;
    private LoginService _loginService;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new UserProfile()));
        _mapper = mapperConfig.CreateMapper();

        _refreshTokenService = new Mock<IRefreshTokenService>();
        _userRepository = new Mock<IUserRepository>();

        _loginService = new LoginService(_userRepository.Object, _mapper, _refreshTokenService.Object);
    }

    [Test]
    public void LoginUserAsync_WhenUserNotFound_ThrowsUserNotFoundException()
    {
        // Arrange
        var loginUser = _fixture.Create<LoginRequestDto>();

        _userRepository.Setup(x => x.GetUserByEmailAsync(loginUser.Email)).ReturnsAsync(null as User);

        // Act and Assert
        Assert.ThrowsAsync<UserNotFoundException>(async () => await _loginService.LoginUserAsync(loginUser));
    }

    [Test]
    public void LoginUserAsync_WhenUserIsNotActivated_ThrowsEmailVerificationException()
    {
        // Arrange
        var loginUser = _fixture.Create<LoginRequestDto>();
        var user = _fixture.Build<User>().With(x => x.IsAccountActivated, false).Create();

        _userRepository.Setup(x => x.GetUserByEmailAsync(loginUser.Email)).ReturnsAsync(user);

        // Act and Assert
        Assert.ThrowsAsync<EmailVerificationException>(async () => await _loginService.LoginUserAsync(loginUser));
    }

    [Test]
    public void LoginUserAsync_WhenPasswordIsIncorrect_ThrowsFailedLoginException()
    {
        // Arrange
        var loginUser = _fixture.Create<LoginRequestDto>();
        var user = _fixture.Build<User>().With(x => x.IsAccountActivated, true).With(x => x.Role, RoleEnum.Admin).Create();

        _userRepository.Setup(x => x.GetUserByEmailAsync(loginUser.Email)).ReturnsAsync(user);

        // Act and Assert
        Assert.ThrowsAsync<FailedLoginException>(async () => await _loginService.LoginUserAsync(loginUser));
    }

    [TestCase("6dd2d75fba81ee5e32f275e9c3dbb376af928a2f878b6e07644965f0622f3cc0", "string", "phuLowAEuzh+z3yPtOTU56M4ITXLPU61Sry2Fv8OZZg=")]
    [Test]
    public async Task LoginUserAsync_WhenAllDataIsValid_ReturnsLoginResponseDto(string passwordFromDb, string requestPassword, string salt)
    {
        // Arrange
        var loginUser = _fixture.Create<LoginRequestDto>();
        loginUser.Password = requestPassword;

        var user = _fixture.Build<User>()
            .With(x => x.IsAccountActivated, true)
            .With(x => x.Role, RoleEnum.Admin)
            .With(x => x.Password, passwordFromDb)
            .With(x => x.Salt, salt)
            .Create();

        _userRepository.Setup(x => x.GetUserByEmailAsync(loginUser.Email)).ReturnsAsync(user);
        _refreshTokenService.Setup(x => x.GetRefreshTokenAsync(user.Id)).ReturnsAsync(_fixture.Create<string>());

        // Act
        var result = await _loginService.LoginUserAsync(loginUser);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<LoginResponseDto>());
    }
}
