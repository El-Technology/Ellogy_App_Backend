#pragma warning disable CS8618
namespace UserManager.BLL.Dtos.LoginDtos;

public class LoginRequestDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}
