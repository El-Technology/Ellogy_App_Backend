using UserManager.DAL.Enums;

namespace UserManager.BLL.Dtos.ProfileDto;

public class GetUserProfileDto : UserProfileDto
{
    /// <summary>
    ///     The Stripe customer ID associated with the user.
    /// </summary>
    public string? StripeCustomerId { get; set; }

    /// <summary>
    ///     The account plan assigned to the user.
    /// </summary>
    public AccountPlan? AccountPlan { get; set; }

    /// <summary>
    ///     The JWT token for the user.
    /// </summary>
    public string? Jwt { get; set; }
}