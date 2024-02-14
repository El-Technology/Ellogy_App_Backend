using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManager.BLL.Dtos.ProfileDto;
using UserManager.BLL.Interfaces;
using UserManager.Common.Models.AvatarImage;
using UserManager.Common.Options;

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
    private Guid GetUserIdFromToken()
    {
        var status = Guid.TryParse(User.FindFirst(JwtOptions.UserIdClaimName)?.Value, out var userId);
        if (!status)
            throw new Exception("Taking user id error, try again later");

        return userId;
    }

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
}