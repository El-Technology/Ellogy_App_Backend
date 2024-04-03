using Microsoft.AspNetCore.Mvc;
using UserManager.BLL.Dtos.ExternalDtos;
using UserManager.BLL.Interfaces;
using UserManager.DAL.Enums;

namespace UserManager.Api.Controllers;

/// <summary>
/// Controller for external user operations
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class UserExternalController : Controller
{
    private readonly IUserExternalService _userExternalService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userExternalService"></param>
    public UserExternalController(IUserExternalService userExternalService)
    {
        _userExternalService = userExternalService;
    }

    /// <summary>
    /// Method updates user total points usage in userManager db
    /// </summary>
    /// <param name="updateUsageDto">Model using for updating usage</param>
    /// <returns>200 status code</returns>
    [HttpPost("update-user-total-points-usage")]
    public async Task<IActionResult> UpdateUserTotalPointsUsageAsync([FromBody] UpdateUserTotalPointsUsageDto updateUsageDto)
    {
        await _userExternalService.UpdateUserTotalPointsUsageAsync(updateUsageDto.UserId, updateUsageDto.UsedTokens);
        return Ok();
    }

    /// <summary>
    /// Method gets user total points usage from userManager db
    /// </summary>
    /// <param name="userId">User id</param>
    /// <returns>User total points usage</returns>
    [HttpGet("get-user-total-points-usage")]
    public async Task<IActionResult> GetUserTotalPointsUsageAsync([FromQuery] Guid userId)
    {
        var totalPointsUsage = await _userExternalService.GetUserTotalPointsUsageAsync(userId);
        return Ok(totalPointsUsage);
    }

    /// <summary>
    /// Method gets user by id from userManager db
    /// </summary>
    /// <param name="userId">User id</param>
    /// <returns>User</returns>
    [HttpGet("get-user-by-id")]
    public async Task<IActionResult> GetUserByIdAsync([FromQuery] Guid userId)
    {
        var user = await _userExternalService.GetUserByIdAsync(userId);
        return Ok(user);
    }

    /// <summary>
    /// Method finds user by email prefix in userManager db
    /// </summary>
    /// <param name="emailPrefix">Email prefix</param>
    /// <returns>List of users</returns>
    [HttpGet("find-user-by-email")]
    public async Task<IActionResult> FindUserByEmailAsync([FromQuery] string emailPrefix)
    {
        var users = await _userExternalService.FindUserByEmailAsync(emailPrefix);
        return Ok(users);
    }

    /// <summary>
    /// Method gets users by ids from userManager db
    /// </summary>
    /// <param name="userIds">List of user ids</param>
    /// <returns>List of users</returns>
    [HttpPost("get-users-by-ids")]
    public async Task<IActionResult> GetUsersByIdsAsync([FromBody] List<Guid> userIds)
    {
        var users = await _userExternalService.GetUsersByIdsAsync(userIds);
        return Ok(users);
    }

    /// <summary>
    /// Method gets users by ids with pagination from userManager db
    /// </summary>
    /// <param name="getUsersRequest">Model using for getting users with pagination</param>
    /// <returns>List of users</returns>
    [HttpPost("get-users-by-ids-with-pagination")]
    public async Task<IActionResult> GetUsersByIdsWithPaginationAsync(
        [FromBody] GetUsersByIdsWithPaginationDto getUsersRequest)
    {
        var users = await _userExternalService.GetUsersByIdsWithPaginationAsync(
            getUsersRequest.UserIds, getUsersRequest.PaginationRequest);

        return Ok(users);
    }

    /// <summary>
    /// Method adds stripe customer id to user in userManager db
    /// </summary>
    /// <param name="addStripeCustomerIdDto">Model using for adding stripe customer id</param>
    /// <returns>200 status code</returns>
    [HttpPost("add-stripe-customer-id")]
    public async Task<IActionResult> AddStripeCustomerIdAsync(
        [FromBody] AddStripeCustomerIdDto addStripeCustomerIdDto)
    {
        await _userExternalService.AddStripeCustomerIdAsync(
            addStripeCustomerIdDto.UserId, addStripeCustomerIdDto.CustomerId);

        return Ok();
    }

    /// <summary>
    /// Method removes stripe customer id from user in userManager db
    /// </summary>
    /// <param name="userId">User id</param>
    /// <returns>200 status code</returns>
    [HttpDelete("remove-stripe-customer-id")]
    public async Task<IActionResult> RemoveStripeCustomerIdAsync([FromQuery] Guid userId)
    {
        await _userExternalService.RemoveStripeCustomerIdAsync(userId);
        return Ok();
    }

    /// <summary>
    /// Method updates total purchased tokens in userManager db
    /// </summary>
    /// <param name="userId">User id</param>
    /// <param name="totalPurchasedTokens">Total purchased tokens</param>
    /// <returns>200 status code</returns>
    [HttpGet("update-total-purchased-tokens")]
    public async Task<IActionResult> UpdateTotalPurchasedTokensAsync(
        [FromQuery] Guid userId, [FromQuery] int totalPurchasedTokens)
    {
        await _userExternalService.UpdateTotalPurchasedTokensAsync(userId, totalPurchasedTokens);
        return Ok();
    }

    /// <summary>
    /// Method updates account plan in userManager db
    /// </summary>
    /// <param name="userId">User id</param>
    /// <param name="accountPlan">Account plan</param>
    /// <returns>200 status code</returns>
    [HttpGet("update-account-plan")]
    public async Task<IActionResult> UpdateAccountPlanAsync(
        [FromQuery] Guid userId, [FromQuery] AccountPlan accountPlan)
    {
        await _userExternalService.UpdateAccountPlanAsync(userId, accountPlan);
        return Ok();
    }
}
