using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManager.BLL.Dtos.ProfileDto;
using UserManager.BLL.Dtos.RegisterDtos;
using UserManager.BLL.Interfaces;
using UserManager.Common.Helpers;
using UserManager.Common.Models.AvatarImage;

namespace UserManager.Api.Controllers;

/// <summary>
///     The controller for user profile operations.
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class ProfileController : ControllerBase
{
    private readonly IUserProfileService _userProfileService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProfileController" /> class.
    /// </summary>
    /// <param name="userProfileService"></param>
    public ProfileController(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    /// <summary>
    ///     This method retrieves the user id from the JWT token
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private Guid GetUserIdFromToken() =>
        TokenParseHelper.GetUserId(User);

    /// <summary>
    ///     Deletes the user profile.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("deleteUser")]
    public async Task<IActionResult> DeleteUserProfile(Guid userId)
    {
        var response = await _userProfileService.DeleteUserProfileAsync(userId, GetUserIdFromToken());
        return Ok(response);
    }

    /// <summary>
    ///     Updates the user profile.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="userProfileDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("updateUserProfile")]
    public async Task<IActionResult> UpdateUserProfile(Guid userId, [FromBody] UserProfileDto userProfileDto)
    {
        var response = await _userProfileService.UpdateUserProfileAsync(userId, userProfileDto, GetUserIdFromToken());
        return Ok(response);
    }

    /// <summary>
    ///     Changes the user email (sends email).
    /// </summary>
    /// <param name="sendVerificationEmailDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("changeUserEmail")]
    public async Task<IActionResult> ChangeUserEmail([FromBody] SendVerificationEmailDto sendVerificationEmailDto)
    {
        await _userProfileService.ChangeUserEmailAsync(GetUserIdFromToken(), sendVerificationEmailDto);
        return Ok();
    }

    /// <summary>
    ///     Verifies the user email.
    /// </summary>
    /// <param name="activateUserAccountDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("verifyUserEmail")]
    public async Task<IActionResult> VerifyUserEmail([FromBody] ActivateUserAccountDto activateUserAccountDto)
    {
        await _userProfileService.VerifyUserEmailAsync(GetUserIdFromToken(), activateUserAccountDto);
        return Ok();
    }

    /// <summary>
    ///     Changes the user password.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="changePasswordDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("changeUserPassword")]
    public async Task<IActionResult> ChangeUserPassword(Guid userId, [FromBody] ChangePasswordDto changePasswordDto)
    {
        var response =
            await _userProfileService.ChangeUserPasswordAsync(userId, changePasswordDto, GetUserIdFromToken());
        return Ok(response);
    }

    /// <summary>
    ///     Uploads the user avatar.
    /// </summary>
    /// <param name="uploadAvatar"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("uploadAvatar")]
    public async Task<IActionResult> UploadAvatar(UploadAvatar uploadAvatar)
    {
        var response = await _userProfileService.UploadUserAvatarAsync(uploadAvatar, GetUserIdFromToken());
        return Ok(response);
    }

    /// <summary>
    ///     Gets the user profile.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("getUserProfile")]
    public async Task<IActionResult> GetUserProfile()
    {
        var response = await _userProfileService.GetUserProfileAsync(GetUserIdFromToken());
        return Ok(response);
    }

    /// <summary>
    ///    Gets the user profile by identifier.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("getUserProfileById")]
    public async Task<IActionResult> GetUserProfileById(Guid userId)
    {
        var response = await _userProfileService.GetUserProfileByIdAsync(userId);
        return Ok(response);
    }

    /// <summary>
    ///    Finds the user profile by email.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("findUserProfileByEmail")]
    public async Task<IActionResult> FindUserProfileByEmail(string email)
    {
        var response = await _userProfileService.FindUserByEmailAsync(email);
        return Ok(response);
    }

    /// <summary>
    ///   Gets the user profile by email.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("getUserProfileByEmail")]
    public async Task<IActionResult> GetUserProfileByEmail(string email)
    {
        var response = await _userProfileService.GetUserByEmailAsync(email);
        return Ok(response);
    }
}