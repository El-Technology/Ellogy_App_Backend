using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketsManager.BLL.Dtos.UserStoryTestDtos;
using TicketsManager.BLL.Interfaces;

namespace TicketsManager.Api.Controllers;

/// <summary>
///    Controller for UserStoryTest
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class UserStoryTestController : ControllerBase
{
    private readonly IUserStoryTestService _userStoryTestService;

    /// <summary>
    ///    Constructor for UserStoryTestController
    /// </summary>
    /// <param name="userStoryTestService"></param>
    public UserStoryTestController(IUserStoryTestService userStoryTestService)
    {
        _userStoryTestService = userStoryTestService;
    }

    /// <summary>
    ///     Controller for creating user story tests
    /// </summary>
    /// <param name="userStoryTest"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("createUserStoryTests")]
    public async Task<IActionResult> AddUserStoryTestAsync(
        [FromBody] List<CreateUserStoryTestDto> userStoryTest)
    {
        return Ok(await _userStoryTestService.AddUserStoryTestAsync(userStoryTest));
    }

    /// <summary>
    ///     Controller for getting user story tests
    /// </summary>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("getUserStoryTests")]
    public async Task<IActionResult> GetUserStoryTestsAsync([FromQuery] Guid ticketId)
    {
        return Ok(await _userStoryTestService.GetUserStoryTestsAsync(ticketId));
    }

    /// <summary>
    ///     Controller for updating user story tests
    /// </summary>
    /// <param name="userStoryTest"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("updateUserStoryTests")]
    public async Task<IActionResult> UpdateUserStoryTestAsync(
        [FromBody] List<UpdateUserStoryTestDto> userStoryTest)
    {
        await _userStoryTestService.UpdateUserStoryTestAsync(userStoryTest);
        return Ok();
    }

    /// <summary>
    ///     Controller for deleting user story tests
    /// </summary>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("deleteUserStoryTests")]
    public async Task<IActionResult> DeleteUserStoryTestAsync([FromQuery] Guid ticketId)
    {
        await _userStoryTestService.DeleteUserStoryTestAsync(ticketId);
        return Ok();
    }

    /// <summary>
    ///    Controller for deleting test cases
    /// </summary>
    /// <param name="listOfTestCaseIds"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("deleteTestCases")]
    public async Task<IActionResult> DeleteTestCasesByIds([FromBody] List<Guid> listOfTestCaseIds)
    {
        await _userStoryTestService.DeleteTestCasesByIds(listOfTestCaseIds);
        return Ok();
    }
}