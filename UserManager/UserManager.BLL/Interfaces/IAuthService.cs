using UserManager.BLL.Dtos.LoginDtos;
using UserManager.BLL.Dtos.RegisterDtos;

namespace UserManager.BLL.Interfaces;

public interface IAuthService
{
	public Task<LoginResponseDto> LoginUser(LoginRequestDto loginUser);    
	public Task RegisterUserAsync(UserRegisterRequestDto userRegister);

}
