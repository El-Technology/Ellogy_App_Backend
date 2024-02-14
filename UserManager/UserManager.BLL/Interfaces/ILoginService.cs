using UserManager.BLL.Dtos.LoginDtos;

namespace UserManager.BLL.Interfaces;

public interface ILoginService
{
    /// <summary>
    ///     Login user
    /// </summary>
    /// <param name="loginUser"></param>
    /// <returns></returns>
    public Task<LoginResponseDto> LoginUserAsync(LoginRequestDto loginUser);
}