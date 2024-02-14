using UserManager.BLL.Dtos.RegisterDtos;

namespace UserManager.BLL.Interfaces;

public interface IRegisterService
{
    /// <summary>
    ///     Activates user account.
    /// </summary>
    /// <param name="activateUser"></param>
    /// <returns></returns>
    Task ActivateUserAccountAsync(ActivateUserAccountDto activateUser);

    /// <summary>
    ///     Registers a new user.
    /// </summary>
    /// <param name="userRegister"></param>
    /// <returns></returns>
    public Task RegisterUserAsync(UserRegisterRequestDto userRegister);

    /// <summary>
    ///     Sends verification email.
    /// </summary>
    /// <param name="sendVerificationEmailDto"></param>
    /// <returns></returns>
    Task SendVerificationEmailAsync(SendVerificationEmailDto sendVerificationEmailDto);
}