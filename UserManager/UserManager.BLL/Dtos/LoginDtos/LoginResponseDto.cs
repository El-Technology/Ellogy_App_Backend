using UserManager.DAL.Enums;

#pragma warning disable CS8618
namespace UserManager.BLL.Dtos.LoginDtos;

public class LoginResponseDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Organization { get; set; }
    public string Department { get; set; }
    public string Jwt { get; set; }
    public RoleEnum Role { get; set; }
}
