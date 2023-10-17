namespace UserManager.BLL.Dtos.RegisterDtos;

/// <summary>
/// Represents the data required to register a new user.
/// </summary>
public class UserRegisterRequestDto
{
    /// <summary>
    /// The first name of the user.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// The last name of the user.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// The email address of the user.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// The phone number of the user.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// The password of the user. Will hashed during registration
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// The organization name of the user.
    /// </summary>
    public string? Organization { get; set; }

    /// <summary>
    /// The department name of the user.
    /// </summary>
    public string? Department { get; set; }
}