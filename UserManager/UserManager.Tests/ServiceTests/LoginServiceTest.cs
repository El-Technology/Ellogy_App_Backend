using AutoFixture;
using UserManager.BLL.Dtos.LoginDtos;
using UserManager.BLL.Dtos.RegisterDtos;
using UserManager.BLL.Services;
using UserManager.DAL.Repositories;

namespace UserManager.Tests.ServiceTests
{
    [TestFixture]
    public class LoginServiceTest : BaseClassForServices
    {

        [Test]
        public async Task LoginUserAsyncReturnsResponseDto()
        {
            var refreshTokenRepository = new RefreshTokenRepository(_userManagerDbContext);
            var userRepository = new UserRepository(_userManagerDbContext);
            var refreshTokenService = new RefreshTokenService(refreshTokenRepository, userRepository);
            var registerService = new RegisterService(_mapper, userRepository);

            var loginService = new LoginService(userRepository, _mapper, refreshTokenService);

            //Register new User
            var user = _fixture.Create<UserRegisterRequestDto>();
            await registerService.RegisterUserAsync(user);

            //Try login
            var loginRequestDto = new LoginRequestDto
            {
                Email = user.Email,
                Password = user.Password
            };
            var actualResponse = await loginService.LoginUserAsync(loginRequestDto);

            Assert.Multiple(() =>
            {
                Assert.That(actualResponse.Jwt, Is.Not.Null);
                Assert.That(actualResponse.RefreshToken, Is.Not.Null);
                Assert.That(actualResponse.Email, Is.EqualTo(user.Email));
            });
        }
    }
}
