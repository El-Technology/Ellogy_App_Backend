namespace UserManager.BLL.Dtos.LoginDtos;

/// <summary>
/// Represents the login credentials of a user.
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// The email address of the user.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// The password of the user.
    /// </summary>
    public string Password { get; set; }
}