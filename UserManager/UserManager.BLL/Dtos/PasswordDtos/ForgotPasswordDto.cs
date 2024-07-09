using UserManager.Common.Attributes;

namespace UserManager.BLL.Dtos.PasswordDtos;

public class ForgotPasswordDto
{
    [EmailValidation]
    public string Email { get; set; } = string.Empty;
    public string RedirectUrl { get; set; }
}
