//using AutoFixture;
//using UserManager.BLL.Dtos.LoginDtos;
//using UserManager.BLL.Dtos.RefreshTokenDtos;
//using UserManager.BLL.Dtos.RegisterDtos;
//using UserManager.BLL.Services;
//using UserManager.DAL.Repositories;

//namespace UserManager.Tests.ServiceTests
//{
//    public class RefreshTokenServiceTest : BaseClassForServices
//    {
//        [Test]
//        public async Task GetRefreshTokenAsync_ReturnsRefreshTokenModel()
//        {
//            var refreshTokenRepository = new RefreshTokenRepository(_userManagerDbContext);
//            var userRepository = new UserRepository(_userManagerDbContext);
//            var refreshTokenService = new RefreshTokenService(refreshTokenRepository, userRepository);
//            var registerService = new RegisterService(_mapper, userRepository);

//            var user = _fixture.Create<UserRegisterRequestDto>();
//            await registerService.RegisterUserAsync(user);

//            var userFromDb = await userRepository.GetUserByEmailAsync(user.Email);

//            var refreshToken = await refreshTokenService.GetRefreshTokenAsync(userFromDb.Id);

//            Assert.That(refreshToken, Is.Not.Null);
//        }

//        [Test]
//        public async Task RegenerateJwtAsync_ReturnsNewJwtToken()
//        {
//            var refreshTokenRepository = new RefreshTokenRepository(_userManagerDbContext);
//            var userRepository = new UserRepository(_userManagerDbContext);
//            var refreshTokenService = new RefreshTokenService(refreshTokenRepository, userRepository);
//            var registerService = new RegisterService(_mapper, userRepository);
//            var loginService = new LoginService(userRepository, _mapper, refreshTokenService);

//            //Register new user
//            var user = _fixture.Create<UserRegisterRequestDto>();
//            await registerService.RegisterUserAsync(user);

//            //Login to get old jwt and refresh tokens
//            var loginResponse = await loginService.LoginUserAsync(new LoginRequestDto { Email = user.Email, Password = user.Password });

//            var refreshTokenModel = new RefreshTokenRequestDto
//            {
//                Jwt = loginResponse.Jwt,
//                RefreshToken = loginResponse.RefreshToken,
//            };
//            var newJwtToken = await refreshTokenService.RegenerateJwtAsync(refreshTokenModel);

//            Assert.That(newJwtToken, Is.Not.Null);
//        }
//    }
//}
