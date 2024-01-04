using UserManager.BLL.Dtos.RegisterDtos;

namespace UserManager.BLL.Interfaces;

public interface IRegisterService
{
    Task ActivateUserAccountAsync(ActivateUserAccountDto activateUser);
    public Task RegisterUserAsync(UserRegisterRequestDto userRegister);
    Task SendVerificationEmailAsync(SendVerificationEmailDto sendVerificationEmailDto);
}