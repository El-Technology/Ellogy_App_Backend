//using AutoFixture;
//using Azure;
//using Azure.Storage.Blobs;
//using Azure.Storage.Blobs.Models;
//using Moq;
//using System.Text;
//using UserManager.BLL.Dtos.LoginDtos;
//using UserManager.BLL.Dtos.ProfileDto;
//using UserManager.BLL.Dtos.RegisterDtos;
//using UserManager.BLL.Services;
//using UserManager.Common.Models.AvatarImage;
//using UserManager.DAL.Repositories;

//namespace UserManager.Tests.ServiceTests
//{
//    [TestFixture]
//    public class UserProfileTest : BaseClassForServices
//    {
//        [Test]
//        public async Task UpdateUserProfileAsync_ChangeDataInDb()
//        {
//            var userRepository = new UserRepository(_userManagerDbContext);
//            var registerService = new RegisterService(_mapper, userRepository);
//            var blobServiceClientMock = new Mock<BlobServiceClient>();
//            var containerClientMock = new Mock<BlobContainerClient>();
//            var blobContentInfoMock = new Mock<BlobContentInfo>();
//            var blobClientMock = new Mock<BlobClient>();
//            var responseMock = new Mock<Response>();

//            //Blob configuration
//            blobClientMock.Setup(x => x.UploadAsync(It.IsAny<Stream>(), true, default))
//                .ReturnsAsync(Response.FromValue(blobContentInfoMock.Object, responseMock.Object));
//            containerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);
//            blobServiceClientMock.Setup(client => client.GetBlobContainerClient(It.IsAny<string>())).Returns(containerClientMock.Object);

//            var userProfileService = new UserProfileService(userRepository, _mapper, blobServiceClientMock.Object);

//            //Register new user
//            var user = _fixture.Create<UserRegisterRequestDto>();
//            await registerService.RegisterUserAsync(user);
//            var userFromDb = await userRepository.GetUserByEmailAsync(user.Email);

//            //Update user profile
//            var userProfileDto = _fixture.Create<UserProfileDto>();
//            await userProfileService.UpdateUserProfileAsync(userFromDb.Id, userProfileDto, userFromDb.Id);

//            //Take updated data
//            var updatedUserFromDb = await userRepository.GetUserByEmailAsync(user.Email);

//            Assert.Multiple(() =>
//            {
//                Assert.That(user.FirstName, Is.Not.EqualTo(updatedUserFromDb?.FirstName));
//                Assert.That(user.LastName, Is.Not.EqualTo(updatedUserFromDb?.LastName));
//            });
//        }

//        [Test]
//        public async Task DeleteUserProfileAsync_DeletesUserFromDb()
//        {
//            var userRepository = new UserRepository(_userManagerDbContext);
//            var registerService = new RegisterService(_mapper, userRepository);
//            var blobServiceClientMock = new Mock<BlobServiceClient>();
//            var containerClientMock = new Mock<BlobContainerClient>();
//            var blobContentInfoMock = new Mock<BlobContentInfo>();
//            var blobClientMock = new Mock<BlobClient>();
//            var responseMock = new Mock<Response>();

//            //Blob configuration
//            blobClientMock.Setup(x => x.UploadAsync(It.IsAny<Stream>(), true, default))
//                .ReturnsAsync(Response.FromValue(blobContentInfoMock.Object, responseMock.Object));
//            containerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);
//            blobServiceClientMock.Setup(client => client.GetBlobContainerClient(It.IsAny<string>())).Returns(containerClientMock.Object);

//            var userProfileService = new UserProfileService(userRepository, _mapper, blobServiceClientMock.Object);

//            //Register new user
//            var user = _fixture.Create<UserRegisterRequestDto>();
//            await registerService.RegisterUserAsync(user);
//            var userFromDb = await userRepository.GetUserByEmailAsync(user.Email);

//            //Delete user
//            var result = await userProfileService.DeleteUserProfileAsync(userFromDb.Id, userFromDb.Id);
//            var deletedUser = await userRepository.GetUserByEmailAsync(user.Email);

//            Assert.Multiple(() =>
//            {
//                Assert.That(result, Is.True);
//                Assert.That(deletedUser, Is.Null);
//            });
//        }

//        [Test]
//        public async Task ChangeUserPasswordAsync_ChangesUserPassword()
//        {
//            var userRepository = new UserRepository(_userManagerDbContext);
//            var registerService = new RegisterService(_mapper, userRepository);
//            var blobServiceClientMock = new Mock<BlobServiceClient>();
//            var containerClientMock = new Mock<BlobContainerClient>();
//            var blobContentInfoMock = new Mock<BlobContentInfo>();
//            var blobClientMock = new Mock<BlobClient>();
//            var responseMock = new Mock<Response>();
//            var refreshTokenRepository = new RefreshTokenRepository(_userManagerDbContext);
//            var refreshTokenService = new RefreshTokenService(refreshTokenRepository, userRepository);
//            var loginService = new LoginService(userRepository, _mapper, refreshTokenService);

//            //Blob configuration
//            blobClientMock.Setup(x => x.UploadAsync(It.IsAny<Stream>(), true, default))
//                .ReturnsAsync(Response.FromValue(blobContentInfoMock.Object, responseMock.Object));
//            containerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);
//            blobServiceClientMock.Setup(client => client.GetBlobContainerClient(It.IsAny<string>())).Returns(containerClientMock.Object);

//            var userProfileService = new UserProfileService(userRepository, _mapper, blobServiceClientMock.Object);

//            //Register new user
//            var user = _fixture.Create<UserRegisterRequestDto>();
//            await registerService.RegisterUserAsync(user);
//            var userFromDb = await userRepository.GetUserByEmailAsync(user.Email);

//            //Change password
//            var changePasswordDto = new ChangePasswordDto
//            {
//                oldPassword = user.Password,
//                newPassword = "NewPassword",
//            };
//            await userProfileService.ChangeUserPasswordAsync(userFromDb.Id, changePasswordDto, userFromDb.Id);

//            //Try login with new password
//            var loginResponse = await loginService.LoginUserAsync(new LoginRequestDto { Email = user.Email, Password = changePasswordDto.newPassword });

//            Assert.Multiple(() =>
//            {
//                Assert.That(loginResponse, Is.Not.Null);
//                Assert.That(loginResponse.Email, Is.EqualTo(user.Email));
//            });
//        }

//        [Test]
//        public async Task UploadUserAvatarAsync_SetsNewLinkToAvatar()
//        {
//            var userRepository = new UserRepository(_userManagerDbContext);
//            var registerService = new RegisterService(_mapper, userRepository);
//            var blobServiceClientMock = new Mock<BlobServiceClient>();
//            var containerClientMock = new Mock<BlobContainerClient>();
//            var blobContentInfoMock = new Mock<BlobContentInfo>();
//            var blobClientMock = new Mock<BlobClient>();
//            var responseMock = new Mock<Response>();
//            var refreshTokenRepository = new RefreshTokenRepository(_userManagerDbContext);
//            var refreshTokenService = new RefreshTokenService(refreshTokenRepository, userRepository);
//            var loginService = new LoginService(userRepository, _mapper, refreshTokenService);

//            //Blob configuration
//            blobClientMock.Setup(x => x.UploadAsync(It.IsAny<Stream>(), true, default))
//                .ReturnsAsync(Response.FromValue(blobContentInfoMock.Object, responseMock.Object));
//            blobClientMock.Setup(x => x.Uri).Returns(new Uri("https://LinkToAvatar"));
//            containerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);
//            blobServiceClientMock.Setup(client => client.GetBlobContainerClient(It.IsAny<string>())).Returns(containerClientMock.Object);

//            var userProfileService = new UserProfileService(userRepository, _mapper, blobServiceClientMock.Object);

//            //Register new user
//            var user = _fixture.Create<UserRegisterRequestDto>();
//            await registerService.RegisterUserAsync(user);
//            var userFromDb = await userRepository.GetUserByEmailAsync(user.Email);

//            //Change avatar
//            var bytesForBase64 = Encoding.UTF8.GetBytes(_fixture.Create<string>());
//            var base64String = Convert.ToBase64String(bytesForBase64);
//            var avatarModel = new UploadAvatar
//            {
//                Base64Avatar = base64String,
//                ImageExtension = ImageExtensions.Jpg,
//                UserId = userFromDb.Id
//            };
//            var avatarLink = await userProfileService.UploadUserAvatarAsync(avatarModel, userFromDb.Id);

//            //Try login with new avatar
//            var loginResponse = await loginService.LoginUserAsync(new LoginRequestDto { Email = user.Email, Password = user.Password });

//            Assert.Multiple(() =>
//            {
//                Assert.That(avatarLink, Is.Not.Null);
//                Assert.That(loginResponse.AvatarLink, Is.EqualTo(avatarLink));
//            });
//        }
//    }
//}
