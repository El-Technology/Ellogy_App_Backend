using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketsManager.BLL.Dtos.ActionHistoryDtos;
using TicketsManager.BLL.Interfaces;

namespace TicketsManager.Api.Controllers;

/// <summary>
/// Controller for ActionHistory
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class ActionHistoryController : Controller
{
    private readonly IActionHistoryService _actionHistoryService;

    /// <summary>
    /// Constructor for ActionHistoryController
    /// </summary>
    /// <param name="actionHistoryService"></param>
    public ActionHistoryController(IActionHistoryService actionHistoryService)
    {
        _actionHistoryService = actionHistoryService;
    }

    /// <summary>
    /// Create action history
    /// </summary>
    /// <param name="createActionHistoryDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("createActionHistory")]
    public async Task<IActionResult> CreateActionHistory(
        [FromBody] CreateActionHistoryDto createActionHistoryDto)
    {
        await _actionHistoryService.CreateActionHistoryAsync(createActionHistoryDto);

        return Ok(createActionHistoryDto);
    }

    /// <summary>
    /// Get action history
    /// </summary>
    /// <param name="TicketId"></param>
    /// <param name="searchHistoryRequestDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("getActionHistory")]
    public async Task<IActionResult> GetActionHistory(
        Guid TicketId, [FromBody] SearchHistoryRequestDto searchHistoryRequestDto)
    {
        var result = await _actionHistoryService.GetActionHistoriesAsync(
            TicketId, searchHistoryRequestDto);

        return Ok(result);
    }
}
