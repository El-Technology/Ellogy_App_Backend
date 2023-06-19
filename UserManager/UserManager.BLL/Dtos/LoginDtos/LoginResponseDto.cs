using UserManager.DAL.Enums;

namespace UserManager.BLL.Dtos.LoginDtos
{
    /// <summary>
    /// Represents the response data after a successful login.
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// The unique identifier of the user.
        /// </summary>
        public Guid Id { get; set; }

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
        public string PhoneNumber { get; set; }

        /// <summary>
        /// The organization name of the user.
        /// </summary>
        public string Organization { get; set; }

        /// <summary>
        /// The department name of the user.
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// The JSON Web Token (JWT) generated for the user.
        /// </summary>
        public string Jwt { get; set; }

        /// <summary>
        /// The role assigned to the user.
        /// </summary>
        public RoleEnum Role { get; set; }
    }
}
