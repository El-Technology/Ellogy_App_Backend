using UserManager.BLL.Dtos.PasswordDtos;

namespace UserManager.BLL.Interfaces;

public interface IPasswordService
{
    public Task<string> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
    public Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
}
