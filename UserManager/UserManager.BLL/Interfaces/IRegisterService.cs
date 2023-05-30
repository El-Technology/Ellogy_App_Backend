using UserManager.BLL.Dtos;

namespace UserManager.BLL.Interfaces;

public interface IRegisterService
{
    public Task<UserRegisterResponseDto> RegisterUser(UserRegisterRequestDto userRegister);
}