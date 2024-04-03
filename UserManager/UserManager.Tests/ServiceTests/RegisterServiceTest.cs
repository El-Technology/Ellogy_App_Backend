using AutoFixture;
using AutoMapper;
using Moq;
using UserManager.BLL.Dtos.RegisterDtos;
using UserManager.BLL.Exceptions;
using UserManager.BLL.Interfaces;
using UserManager.BLL.Mapping;
using UserManager.BLL.Services;
using UserManager.Common.Helpers;
using UserManager.Common.Models.NotificationModels;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.Tests.ServiceTests;

[TestFixture]
public class RegisterServiceTest
{
    private IMapper _mapper;
    private Fixture _fixture;
    private RegisterService _registerService;

    private Mock<IUserRepository> _userRepository;
    private Mock<INotificationQueueService> _notificationQueueService;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _userRepository = new Mock<IUserRepository>();
        _notificationQueueService = new Mock<INotificationQueueService>();
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<UserProfile>()).CreateMapper();
        _registerService = new RegisterService(_mapper, _userRepository.Object, _notificationQueueService.Object);
    }

    [Test]
    public async Task RegisterUserAsync_ShouldAddUserToDatabase_WhenUserDoesNotExist()
    {
        // Arrange
        var userRegisterRequestDto = _fixture.Create<UserRegisterRequestDto>();
        _userRepository.Setup(x => x.CheckEmailIsExistAsync(userRegisterRequestDto.Email)).ReturnsAsync(false);

        // Act
        await _registerService.RegisterUserAsync(userRegisterRequestDto);

        // Assert
        _userRepository.Verify(x => x.AddUserAsync(It.IsAny<User>()), Times.Once);
    }

    [Test]
    public void RegisterUserAsync_ShouldThrowUserAlreadyExistException_WhenUserExists()
    {
        // Arrange
        var userRegisterRequestDto = _fixture.Create<UserRegisterRequestDto>();
        _userRepository.Setup(x => x.CheckEmailIsExistAsync(userRegisterRequestDto.Email)).ReturnsAsync(true);

        // Act and Assert
        Assert.ThrowsAsync<UserAlreadyExistException>(async () => await _registerService.RegisterUserAsync(userRegisterRequestDto));
    }

    [Test]
    public async Task ActivateUserAccountAsync_ShouldActivateUserAccount_WhenUserExists()
    {
        // Arrange
        var user = _fixture.Create<User>();
        var activateUser = _fixture.Create<ActivateUserAccountDto>();
        activateUser.UserEmail = user.Email;
        var verifyToken = CryptoHelper.GenerateToken();
        activateUser.Token = CryptoHelper.GetHash(verifyToken);
        user.VerifyToken = verifyToken;

        _userRepository.Setup(x => x.GetUserByEmailAsync(activateUser.UserEmail)).ReturnsAsync(user);
        _userRepository.Setup(x => x.UpdateUserAsync(user)).Returns(Task.CompletedTask);

        // Act
        await _registerService.ActivateUserAccountAsync(activateUser);

        // Assert
        Assert.That(user.IsAccountActivated, Is.True);
        _userRepository.Verify(x => x.UpdateUserAsync(user), Times.Once);
    }

    [Test]
    public void ActivateUserAccountAsync_ShouldThrowUserNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var activateUser = _fixture.Create<ActivateUserAccountDto>();
        _userRepository.Setup(x => x.GetUserByEmailAsync(activateUser.UserEmail)).ReturnsAsync(null as User);

        // Act and Assert
        Assert.ThrowsAsync<UserNotFoundException>(async () => await _registerService.ActivateUserAccountAsync(activateUser));
    }

    [Test]
    public void SendVerificationEmailAsync_ShouldThrowUserNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var sendVerificationEmailDto = _fixture.Create<SendVerificationEmailDto>();
        _userRepository.Setup(x => x.GetUserByEmailAsync(sendVerificationEmailDto.UserEmail)).ReturnsAsync(null as User);

        // Act and Assert
        Assert.ThrowsAsync<UserNotFoundException>(async () => await _registerService.SendVerificationEmailAsync(sendVerificationEmailDto));
    }

    [Test]
    public void SendVerificationEmailAsync_ShouldThrowException_WhenVerifyTokenIsNotFound()
    {
        // Arrange
        var sendVerificationEmailDto = _fixture.Create<SendVerificationEmailDto>();
        var user = _fixture.Create<User>();
        user.VerifyToken = null;
        _userRepository.Setup(x => x.GetUserByEmailAsync(sendVerificationEmailDto.UserEmail)).ReturnsAsync(user);

        // Act and Assert
        Assert.ThrowsAsync<Exception>(async () => await _registerService.SendVerificationEmailAsync(sendVerificationEmailDto));
    }

    [Test]
    public async Task SendVerificationEmailAsync_ShouldSendEmail_WhenUserExists()
    {
        // Arrange
        var sendVerificationEmailDto = _fixture.Create<SendVerificationEmailDto>();
        var user = _fixture.Create<User>();
        _userRepository.Setup(x => x.GetUserByEmailAsync(sendVerificationEmailDto.UserEmail)).ReturnsAsync(user);

        // Act
        await _registerService.SendVerificationEmailAsync(sendVerificationEmailDto);

        // Assert
        _notificationQueueService.Verify(x => x.SendNotificationAsync(It.IsAny<NotificationModel>()), Times.Once);
    }
}
