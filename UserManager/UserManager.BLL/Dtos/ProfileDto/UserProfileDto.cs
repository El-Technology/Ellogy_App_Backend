namespace UserManager.BLL.Dtos.ProfileDto;

public class UserProfileDto
{
    /// <summary>
    ///     The first name of the user.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    ///     The last name of the user.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    ///     The email address of the user.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    ///     The phone number of the user.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    ///     The organization name of the user.
    /// </summary>
    public string? Organization { get; set; }

    /// <summary>
    ///     The department name of the user.
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    ///     The link to the user's avatar image.
    /// </summary>
    public string? AvatarLink { get; set; }
}