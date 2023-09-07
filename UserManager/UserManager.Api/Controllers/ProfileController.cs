using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManager.BLL.Dtos.ProfileDto;
using UserManager.BLL.Interfaces;
using UserManager.Common.Models.AvatarImage;

namespace UserManager.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        public ProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpDelete]
        [Route("DeleteUser")]
        public async Task<IActionResult> GetUserProfile(Guid userId)
        {
            var response = await _userProfileService.DeleteUserProfileAsync(userId);
            return Ok(response);
        }

        [HttpPost]
        [Route("updateUserProfile")]
        public async Task<IActionResult> UpdateUserProfile(Guid userId, [FromBody] UserProfileDto userProfileDto)
        {
            var response = await _userProfileService.UpdateUserProfileAsync(userId, userProfileDto);
            return Ok(response);
        }

        [HttpPost]
        [Route("changeUserPassword")]
        public async Task<IActionResult> ChangeUserPassword(Guid userId, [FromBody] ChangePasswordDto changePasswordDto)
        {
            var response = await _userProfileService.ChangeUserPasswordAsync(userId, changePasswordDto);
            return Ok(response);
        }

        [HttpPost]
        [Route("UploadAvatar")]
        public async Task<IActionResult> UploadAvatar(UploadAvatar uploadAvatar)
        {
            var response = await _userProfileService.UploadUserAvatarAsync(uploadAvatar);
            return Ok(response);
        }
    }
}
