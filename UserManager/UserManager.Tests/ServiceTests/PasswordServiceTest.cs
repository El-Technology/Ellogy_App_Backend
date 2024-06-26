using AutoFixture;
using Moq;
using System.Web;
using UserManager.BLL.Dtos.PasswordDtos;
using UserManager.BLL.Exceptions;
using UserManager.BLL.Interfaces;
using UserManager.BLL.Services;
using UserManager.Common.Models.NotificationModels;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.Tests.ServiceTests;

[TestFixture]
public class PasswordServiceTest
{
    private Mock<IForgotPasswordRepository> _forgotPasswordRepository;
    private Mock<IExternalNotificationService> _notificationQueueService;
    private Mock<IUserRepository> _userRepository;
    private PasswordService _passwordService;
    private Fixture _fixture = new();

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _forgotPasswordRepository = new Mock<IForgotPasswordRepository>();
        _notificationQueueService = new Mock<IExternalNotificationService>();
        _userRepository = new Mock<IUserRepository>();
        _passwordService = new PasswordService(
            _userRepository.Object, _forgotPasswordRepository.Object, _notificationQueueService.Object);
    }

    [Test]
    public void ForgotPasswordAsync_ShouldThrowUserNotFoundException_WhenUserNotFound()
    {
        // Arrange
        var forgotPasswordDto = _fixture.Create<ForgotPasswordDto>();
        _userRepository.Setup(x => x.CheckEmailIsExistAsync(forgotPasswordDto.Email)).ReturnsAsync(false);

        // Act and Assert
        Assert.ThrowsAsync<UserNotFoundException>(async () => await _passwordService.ForgotPasswordAsync(forgotPasswordDto));
    }

    [Test]
    public async Task ForgotPasswordAsync_ShouldAddForgotPasswordEntry_WhenUserFound()
    {
        // Arrange
        var forgotPasswordDto = _fixture.Create<ForgotPasswordDto>();
        var user = _fixture.Create<User>();
        _userRepository.Setup(x => x.CheckEmailIsExistAsync(forgotPasswordDto.Email)).ReturnsAsync(true);
        _userRepository.Setup(x => x.GetUserByEmailAsync(forgotPasswordDto.Email)).ReturnsAsync(user);

        // Act
        await _passwordService.ForgotPasswordAsync(forgotPasswordDto);

        // Assert
        _forgotPasswordRepository.Verify(x => x.AddForgotTokenAsync(It.IsAny<ForgotPassword>()), Times.Once);
    }

    [Test]
    public async Task ForgotPasswordAsync_ShouldSendNotification_WhenUserFound()
    {
        // Arrange
        var forgotPasswordDto = _fixture.Create<ForgotPasswordDto>();
        var user = _fixture.Create<User>();
        _userRepository.Setup(x => x.CheckEmailIsExistAsync(forgotPasswordDto.Email)).ReturnsAsync(true);
        _userRepository.Setup(x => x.GetUserByEmailAsync(forgotPasswordDto.Email)).ReturnsAsync(user);

        // Act
        await _passwordService.ForgotPasswordAsync(forgotPasswordDto);

        // Assert
        _notificationQueueService.Verify(x => x.SendNotificationAsync(It.IsAny<NotificationModel>()), Times.Once);
    }

    [Test]
    public async Task ResetPasswordAsync_ShouldResetPassword_WhenValidResetRequest()
    {
        // Arrange
        var resetPasswordDto = _fixture.Create<ResetPasswordDto>();
        var user = _fixture.Create<User>();

        resetPasswordDto.Token = HttpUtility.UrlEncode(resetPasswordDto.Token);
        _forgotPasswordRepository.Setup(x => x.ValidateResetRequestAsync(resetPasswordDto.Id, resetPasswordDto.Token)).ReturnsAsync(true);
        _userRepository.Setup(x => x.GetUserByForgetPasswordIdAsync(resetPasswordDto.Id)).ReturnsAsync(user);

        // Act
        await _passwordService.ResetPasswordAsync(resetPasswordDto);

        // Assert
        _userRepository.Verify(x => x.UpdateUserAsync(user), Times.Once);
        _forgotPasswordRepository.Verify(x => x.InvalidateTokenAsync(resetPasswordDto.Id), Times.Once);
    }
}
