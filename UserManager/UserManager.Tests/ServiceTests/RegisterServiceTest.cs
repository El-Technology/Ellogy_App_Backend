using AutoFixture;
using AutoMapper;
using Moq;
using System.Text.RegularExpressions;
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
    private Mock<IServiceBusQueue> _notificationQueueService;

    private static readonly Regex EmailRegex = new Regex(
        @"^(([^<>()\[\]\\.,;:\s@\""]+(\.[^<>()\[\]\\.,;:\s@\""]+)*)|("".+""))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z0-9]+[-a-zA-Z0-9]*[a-zA-Z0-9]+\.)+[a-zA-Z]{2,}))$",
        RegexOptions.Compiled);

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        return EmailRegex.IsMatch(email);
    }

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _userRepository = new Mock<IUserRepository>();
        _notificationQueueService = new Mock<IServiceBusQueue>();
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<UserProfile>()).CreateMapper();
        _registerService = new RegisterService(_mapper, _userRepository.Object, _notificationQueueService.Object);
    }

    //[Test]
    //public async Task RegisterUserAsync_ShouldAddUserToDatabase_WhenUserDoesNotExist()
    //{
    //    // Arrange
    //    var userRegisterRequestDto = _fixture.Create<UserRegisterRequestDto>();
    //    userRegisterRequestDto.Email = "test@domain.com";

    //    _userRepository.Setup(x => x.CheckEmailIsExistAsync(userRegisterRequestDto.Email)).ReturnsAsync(false);

    //    // Act
    //    await _registerService.RegisterUserAsync(userRegisterRequestDto);

    //    // Assert
    //    _userRepository.Verify(x => x.AddUserAsync(It.IsAny<User>()), Times.Once);
    //}

    [Test]
    public void RegisterUserAsync_ShouldThrowUserAlreadyExistException_WhenUserExists()
    {
        // Arrange
        var userRegisterRequestDto = _fixture.Create<UserRegisterRequestDto>();
        userRegisterRequestDto.Email = "test@domain.com";

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
        _notificationQueueService.Verify(x => x.SendMessageAsync(It.IsAny<NotificationModel>()), Times.Once);
    }

    [TestCase("username@example.com")]
    [TestCase("user.name@example.com")]
    [TestCase("username@example.com")]
    [TestCase("username+label@example.com")]
    [TestCase("username123@example.com")]
    [TestCase("user.name+label@example.co.uk")]
    [TestCase("user-name@example.com")]
    [TestCase("username@example-domain.com")]
    [TestCase("user.name+123@example.org")]
    [TestCase("username123@example.net")]
    [TestCase("username@example.travel")]
    [TestCase("username@subdomain.example.com")]
    [TestCase("user.name@sub.domain.com")]
    [TestCase("user-name@sub-domain.example.com")]
    [TestCase("username@[123.456.789.012]")]
    [TestCase("user.name@[123.456.789.012]")]
    [TestCase("user+name@example.museum")]
    [TestCase("user.name@example.travel")]
    [Test]
    public void EmailValidation_ShouldReturnTrue_WhenEmailIsValid(string emailAddress)
    {
        if (IsValidEmail(emailAddress))
            Assert.Pass();
        else
            Assert.Fail();
    }

    [TestCase("plainaddress")]
    [TestCase("@missingusername.com")]
    [TestCase("username@.com")]
    [TestCase("username@com")]
    [TestCase("username@domain..com")]
    [TestCase("username@-domain.com")]
    [TestCase("username@domain-.com")]
    [TestCase("username@domain.com-")]
    [TestCase("username@domain.com.")]
    [TestCase(".username@domain.com")]
    [TestCase("username@domain..com")]
    [TestCase("username@domain.com..")]
    [TestCase("user@name@domain.com")]
    [TestCase("username@domain@com")]
    [TestCase("username@[111.222.333.44444]")]
    [TestCase("username@domain..com")]
    [TestCase("username@domain.-com")]
    [TestCase("user name@domain.com")]
    [TestCase("username@domain_.com")]
    [TestCase("username@domain.com-")]
    [TestCase("username@localserver")]
    [TestCase("username@localhost")]
    [Test]
    public void EmailValidation_ShouldReturnTrue_WhenEmailIsNotValid(string emailAddress)
    {
        if (IsValidEmail(emailAddress))
            Assert.Fail();
        else
            Assert.Pass();
    }
}
