using UserManager.Common.Attributes;

namespace UserManager.BLL.Dtos.PasswordDtos;

public class ResetPasswordDto
{
    public Guid Id { get; set; }
    public string Token { get; set; }

    [PasswordValidation]
    public string Password { get; set; } = string.Empty;
}
