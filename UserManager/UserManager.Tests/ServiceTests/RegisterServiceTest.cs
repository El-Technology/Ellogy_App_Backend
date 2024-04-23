//using AutoFixture;
//using UserManager.BLL.Dtos.LoginDtos;
//using UserManager.BLL.Dtos.RegisterDtos;
//using UserManager.BLL.Services;
//using UserManager.DAL.Repositories;

//namespace UserManager.Tests.ServiceTests
//{
//    [TestFixture]
//    public class RegisterServiceTest : BaseClassForServices
//    {

//        [Test]
//        public async Task RegisterNewUser_LoginAllowed()
//        {
//            var userRepository = new UserRepository(_userManagerDbContext);
//            var registerService = new RegisterService(_mapper, userRepository);
//            var refreshTokenRepository = new RefreshTokenRepository(_userManagerDbContext);
//            var refreshTokenService = new RefreshTokenService(refreshTokenRepository, userRepository);
//            var loginService = new LoginService(userRepository, _mapper, refreshTokenService);

//            //Register new user
//            var user = _fixture.Create<UserRegisterRequestDto>();
//            await registerService.RegisterUserAsync(user);

//            //Try to login
//            var loginResponse = await loginService.LoginUserAsync(new LoginRequestDto { Email = user.Email, Password = user.Password });

//            Assert.IsNotNull(loginResponse);
//        }
//    }
//}
