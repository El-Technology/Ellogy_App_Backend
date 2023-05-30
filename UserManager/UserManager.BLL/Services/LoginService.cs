using UserManager.BLL.Exceptions;
using UserManager.BLL.Helpers;
using UserManager.BLL.Interfaces;
using UserManager.DAL.Interfaces;

namespace UserManager.BLL.Services;

public class LoginService : ILoginService
{
    private readonly IUserRepository _userRepository;

    public LoginService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<string> LoginUser(string email, string password)
    {
        var user = await _userRepository.GetUserByEmailAsync(email) ?? throw new UserNotFoundException(email);

        if (!CryptoHelper.ConfirmPassword(password, user.Salt, user.Password))
            throw new FailedLoginException();

        return JwtHelper.GenerateJwt(user);
    }
}
