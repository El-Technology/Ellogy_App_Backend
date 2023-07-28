using UserManager.BLL.Dtos.LoginDtos;

namespace UserManager.BLL.Interfaces;

public interface ILoginService
{
    public Task<LoginResponseDto> LoginUser(LoginRequestDto loginUser);
}