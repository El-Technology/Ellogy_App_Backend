using Microsoft.AspNetCore.Mvc;
using UserManager.BLL.Dtos.ExternalDtos;
using UserManager.BLL.Services;

namespace UserManager.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UserExternalController : Controller
{
    private readonly UserExternalService _userExternalService;
    public UserExternalController(UserExternalService userExternalService)
    {
        _userExternalService = userExternalService;
    }

    [HttpPost("update-user-total-points-usage")]
    public async Task<IActionResult> UpdateUserTotalPointsUsageAsync([FromBody] UpdateUserTotalPointsUsageDto updateUsageDto)
    {
        await _userExternalService.UpdateUserTotalPointsUsageAsync(updateUsageDto.UserId, updateUsageDto.UsedTokens);
        return Ok();
    }

    [HttpGet("get-user-total-points-usage")]
    public async Task<IActionResult> GetUserTotalPointsUsageAsync([FromQuery] Guid userId)
    {
        var totalPointsUsage = await _userExternalService.GetUserTotalPointsUsageAsync(userId);
        return Ok(totalPointsUsage);
    }

    [HttpGet("get-user-by-id")]
    public async Task<IActionResult> GetUserByIdAsync([FromQuery] Guid userId)
    {
        var user = await _userExternalService.GetUserByIdAsync(userId);
        return Ok(user);
    }

    [HttpGet("find-user-by-email")]
    public async Task<IActionResult> FindUserByEmailAsync([FromQuery] string emailPrefix)
    {
        var users = await _userExternalService.FindUserByEmailAsync(emailPrefix);
        return Ok(users);
    }

    [HttpPost("get-users-by-ids")]
    public async Task<IActionResult> GetUsersByIdsAsync([FromBody] List<Guid> userIds)
    {
        var users = await _userExternalService.GetUsersByIdsAsync(userIds);
        return Ok(users);
    }

    [HttpPost("get-users-by-ids-with-pagination")]
    public async Task<IActionResult> GetUsersByIdsWithPaginationAsync([FromBody] GetUsersByIdsWithPaginationDto getUsersRequest)
    {
        var users = await _userExternalService.GetUsersByIdsWithPaginationAsync(getUsersRequest.UserIds, getUsersRequest.PaginationRequest);
        return Ok(users);
    }
}
