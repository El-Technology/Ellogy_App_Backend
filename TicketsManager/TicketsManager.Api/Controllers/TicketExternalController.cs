using Microsoft.AspNetCore.Mvc;
using TicketsManager.BLL.Interfaces;
using TicketsManager.DAL.Enums;

namespace TicketsManager.Api.Controllers;

/// <summary>
/// Ticket external controller
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class TicketExternalController : Controller
{
    private readonly ITicketShareExternalService _ticketShareExternalService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="ticketShareExternalService"></param>
    public TicketExternalController(ITicketShareExternalService ticketShareExternalService)
    {
        _ticketShareExternalService = ticketShareExternalService;
    }

    /// <summary>
    /// Check if user have access to component by ticket id
    /// </summary>
    /// <param name="ticketId"></param>
    /// <param name="userId"></param>
    /// <param name="currentStepEnum"></param>
    /// <param name="requireSharePermissionEnum"></param>
    /// <returns></returns>
    [HttpGet("check-user-access")]
    public async Task<IActionResult> CheckIfUserHaveAccessToComponentByTicketIdAsync(
        [FromQuery] Guid ticketId,
        [FromQuery] Guid userId,
        [FromQuery] TicketCurrentStepEnum currentStepEnum,
        [FromQuery] SharePermissionEnum requireSharePermissionEnum)
    {
        await _ticketShareExternalService.CheckIfUserHaveAccessToComponentByTicketIdAsync(
            ticketId,
            userId,
            currentStepEnum,
            requireSharePermissionEnum);

        return Ok();
    }
}
