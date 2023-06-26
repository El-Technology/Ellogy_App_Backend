using UserManager.BLL.Dtos.RegisterDtos;

namespace UserManager.BLL.Interfaces;

public interface IRegisterService
{
    public Task RegisterUserAsync(UserRegisterRequestDto userRegister);
}