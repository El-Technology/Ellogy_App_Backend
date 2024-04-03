using AutoFixture;
using AutoMapper;
using Azure.Storage.Blobs;
using Moq;
using UserManager.BLL.Dtos.ProfileDto;
using UserManager.BLL.Dtos.RegisterDtos;
using UserManager.BLL.Exceptions;
using UserManager.BLL.Interfaces;
using UserManager.BLL.Mapping;
using UserManager.BLL.Services;
using UserManager.Common.Constants;
using UserManager.Common.Helpers;
using UserManager.Common.Models.AvatarImage;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.Tests.ServiceTests;

[TestFixture]
public class UserProfileServiceTest
{
    private Mock<BlobServiceClient> _blobServiceClient;
    private IMapper _mapper;
    private Mock<IRegisterService> _registerService;
    private Mock<IUserRepository> _userRepository;
    private Fixture _fixture;
    private UserProfileService _userProfileService;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _blobServiceClient = new Mock<BlobServiceClient>();
        _registerService = new Mock<IRegisterService>();
        _userRepository = new Mock<IUserRepository>();
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<UserProfile>()).CreateMapper();
        _userProfileService = new UserProfileService(
            _userRepository.Object, _mapper, _blobServiceClient.Object, _registerService.Object);
    }

    private static byte[] GenerateRandomImage()
    {
        var random = new Random();
        var buffer = new byte[1024];
        random.NextBytes(buffer);

        return buffer;
    }

    [Test]
    public async Task UpdateUserProfileAsync_ShouldUpdateUserProfile_WhenUserExists()
    {
        // Arrange
        var userProfileDto = _fixture.Create<UserProfileDto>();
        var userId = Guid.NewGuid();
        var idFromToken = userId;
        var user = _fixture.Create<User>();
        _userRepository.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _userProfileService.UpdateUserProfileAsync(userId, userProfileDto, idFromToken);

        // Assert
        _userRepository.Verify(x => x.UpdateUserAsync(It.IsAny<User>()), Times.Once);
        Assert.That(result, Is.EqualTo(userProfileDto));
    }

    [Test]
    public void UpdateUserProfileAsync_ShouldThrowUserNotFoundException_WhenUserNotFound()
    {
        // Arrange
        var userProfileDto = _fixture.Create<UserProfileDto>();
        var userId = Guid.NewGuid();
        var idFromToken = userId;
        _userRepository.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(null as User);

        // Act and Assert
        Assert.ThrowsAsync<UserNotFoundException>(async () =>
            await _userProfileService.UpdateUserProfileAsync(userId, userProfileDto, idFromToken));
    }

    [Test]
    public async Task DeleteUserProfileAsync_ShouldDeleteUserProfile_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var idFromToken = userId;
        var user = _fixture.Create<User>();
        _userRepository.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(user);

        // Act
        await _userProfileService.DeleteUserProfileAsync(userId, idFromToken);

        // Assert
        _userRepository.Verify(x => x.DeleteUserAsync(user), Times.Once);
    }

    [Test]
    public void DeleteUserProfileAsync_ShouldThrowUserNotFoundException_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var idFromToken = userId;
        _userRepository.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(null as User);

        // Act and Assert
        Assert.ThrowsAsync<UserNotFoundException>(async () =>
                   await _userProfileService.DeleteUserProfileAsync(userId, idFromToken));
    }

    [TestCase("6dd2d75fba81ee5e32f275e9c3dbb376af928a2f878b6e07644965f0622f3cc0", "phuLowAEuzh+z3yPtOTU56M4ITXLPU61Sry2Fv8OZZg=")]
    [Test]
    public async Task ChangeUserPasswordAsync_ShouldChangeUserPassword_WhenUserExists(string password, string salt)
    {
        // Arrange
        var changePasswordDto = _fixture.Create<ChangePasswordDto>();
        changePasswordDto.oldPassword = "string";

        var userId = Guid.NewGuid();
        var idFromToken = userId;
        var user = _fixture.Create<User>();
        user.Password = password;
        user.Salt = salt;

        _userRepository.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(user);

        // Act
        await _userProfileService.ChangeUserPasswordAsync(userId, changePasswordDto, idFromToken);

        // Assert
        _userRepository.Verify(x => x.UpdateUserAsync(It.IsAny<User>()), Times.Once);
    }

    [Test]
    public async Task UploadUserAvatarAsync_ShouldUploadUserAvatar_WhenUserExists()
    {
        // Arrange
        var uploadAvatar = new UploadAvatar
        {
            UserId = Guid.NewGuid(),
            Base64Avatar = Convert.ToBase64String(GenerateRandomImage()),
            ImageExtension = ImageExtensions.Jpg
        };
        var user = new User
        {
            Id = uploadAvatar.UserId
        };
        var idFromToken = uploadAvatar.UserId;

        _userRepository.Setup(x => x.GetUserByIdAsync(uploadAvatar.UserId)).ReturnsAsync(user);

        var containerClientMock = new Mock<BlobContainerClient>();
        _blobServiceClient.Setup(x => x.GetBlobContainerClient(BlobContainerConstants.AvatarsContainer)).Returns(containerClientMock.Object);

        var blobClientMock = new Mock<BlobClient>();
        blobClientMock.SetupGet(x => x.Uri).Returns(new Uri("http://test.com"));
        containerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);

        // Act
        var result = await _userProfileService.UploadUserAvatarAsync(uploadAvatar, idFromToken);

        // Assert
        _userRepository.Verify(x => x.UpdateUserAsync(user), Times.Once);
        Assert.That(user.AvatarLink, Is.EqualTo(result));
    }

    [Test]
    public void GetUserProfileAsync_ShouldReturnProfile_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = _fixture.Create<User>();
        _userRepository.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = _userProfileService.GetUserProfileAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task ChangeUserEmailAsync_ShouldChangeUserEmail_WhenUserExists()
    {
        // Arrange
        var sendVerificationEmailDto = _fixture.Create<SendVerificationEmailDto>();
        var userId = Guid.NewGuid();
        var user = _fixture.Create<User>();
        _userRepository.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(user);

        // Act
        await _userProfileService.ChangeUserEmailAsync(userId, sendVerificationEmailDto);

        // Assert
        _userRepository.Verify(x => x.UpdateUserAsync(It.IsAny<User>()), Times.Once);
    }

    [Test]
    public async Task VerifyUserEmailAsync_ShouldVerifyUserEmail_WhenUserExists()
    {
        // Arrange
        var token = CryptoHelper.GenerateToken();

        var activateUser = _fixture.Create<ActivateUserAccountDto>();
        activateUser.Token = CryptoHelper.GetHash(token);
        var user = _fixture.Create<User>();
        user.VerifyToken = token;
        _userRepository.Setup(x => x.GetUserByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        await _userProfileService.VerifyUserEmailAsync(user.Id, activateUser);

        // Assert
        _userRepository.Verify(x => x.UpdateUserAsync(It.IsAny<User>()), Times.Once);
    }
}
