using UserManager.BLL.Dtos.ProfileDto;
using UserManager.DAL.Enums;

namespace UserManager.BLL.Dtos.LoginDtos
{
    /// <summary>
    /// Represents the response data after a successful login.
    /// </summary>
    public class LoginResponseDto : UserProfileDto
    {
        /// <summary>
        /// The link of user avatar.
        /// </summary>
        public string? AvatarLink { get; set; }

        /// <summary>
        /// The unique identifier of the user.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The JSON Web Token (JWT) generated for the user.
        /// </summary>
        public string Jwt { get; set; }

        /// <summary>
        /// The refresh token for the future requests of refreshing the JWT.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// The role assigned to the user.
        /// </summary>
        public RoleEnum Role { get; set; }

        /// <summary>
        /// The Stripe customer ID associated with the user.
        /// </summary>
        public string? StripeCustomerId { get; set; }

        /// <summary>
        /// The account plan assigned to the user.
        /// </summary>
        public AccountPlan? AccountPlan { get; set; }
    }
}
