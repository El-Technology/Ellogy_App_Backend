using UserManager.BLL.Dtos.ExternalDtos;
using UserManager.BLL.Dtos.ProfileDto;
using UserManager.BLL.Dtos.RegisterDtos;
using UserManager.Common.Models.AvatarImage;

namespace UserManager.BLL.Interfaces;

public interface IUserProfileService
{
    /// <summary>
    ///     Change the password for a user with the specified user ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="changePasswordDto">The new and old password to set for the user.</param>
    /// <param name="idFromToken">User id taken from jwt token</param>
    /// <returns>Returns true if the password is successfully changed; otherwise, false.</returns>
    Task<bool> ChangeUserPasswordAsync(Guid userId, ChangePasswordDto changePasswordDto, Guid idFromToken);

    /// <summary>
    ///     Delete the user profile for a user with the specified user ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="idFromToken">User id taken from jwt token</param>
    /// <returns>Returns true if the user profile is successfully deleted; otherwise, false.</returns>
    Task<bool> DeleteUserProfileAsync(Guid userId, Guid idFromToken);

    /// <summary>
    ///     Retrieves the user profile asynchronously based on the user ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <returns>Task representing the asynchronous operation, returning the user profile DTO</returns>
    Task<GetUserProfileDto> GetUserProfileAsync(Guid userId);

    /// <summary>
    ///     Update the user profile for a user with the specified user ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="userProfileDto">The user profile data to update.</param>
    /// <param name="idFromToken">User id taken from jwt token</param>
    /// <returns>Returns the updated user profile as a UserProfileDto.</returns>
    Task<UserProfileDto> UpdateUserProfileAsync(Guid userId, UserProfileDto userProfileDto, Guid idFromToken);

    /// <summary>
    ///     Upload a user's avatar image.
    /// </summary>
    /// <param name="uploadAvatar">The avatar upload data.</param>
    /// <param name="idFromToken">User id taken from jwt token</param>
    /// <returns>Returns the URL or identifier of the uploaded user avatar.</returns>
    Task<string> UploadUserAvatarAsync(UploadAvatar uploadAvatar, Guid idFromToken);

    /// <summary>
    ///    Change the email for a user with the specified user ID.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="sendVerificationEmailDto"></param>
    /// <returns></returns>
    Task ChangeUserEmailAsync(Guid userId, SendVerificationEmailDto sendVerificationEmailDto);

    /// <summary>
    ///   Verify the email for a user with the specified user ID.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="activateUser"></param>
    /// <returns></returns>
    Task VerifyUserEmailAsync(Guid userId, ActivateUserAccountDto activateUser);

    /// <summary>
    ///    Find users by email prefix asynchronously.
    /// </summary>
    /// <param name="emailPrefix"></param>
    /// <returns></returns>
    Task<List<UserDto>> FindUserByEmailAsync(string emailPrefix);

    /// <summary>
    ///   Get user by email asynchronously.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    Task<UserDto> GetUserByEmailAsync(string email);

    /// <summary>
    ///   Get user profile by id asynchronously.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<UserDto> GetUserProfileByIdAsync(Guid userId);
}