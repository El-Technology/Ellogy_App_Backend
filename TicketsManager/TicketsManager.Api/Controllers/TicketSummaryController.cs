using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketsManager.BLL.Dtos.TicketSummaryDtos;
using TicketsManager.BLL.Interfaces;

namespace TicketsManager.Api.Controllers;

/// <summary>
///     TicketSummaryController
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class TicketSummaryController : ControllerBase
{
    private readonly ITicketSummaryService _ticketSummaryService;

    /// <summary>
    ///     TicketSummaryController
    /// </summary>
    /// <param name="ticketSummaryService"></param>
    public TicketSummaryController(ITicketSummaryService ticketSummaryService)
    {
        _ticketSummaryService = ticketSummaryService;
    }

    /// <summary>
    ///     Get all ticket summaries by ticket id
    /// </summary>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("getTicketSummariesByTicketId")]
    public async Task<IActionResult> GetTicketSummariesByTicketIdAsync([FromQuery] Guid ticketId)
    {
        var ticketSummaries = await _ticketSummaryService.GetTicketSummariesByTicketIdAsync(ticketId);
        return Ok(ticketSummaries);
    }

    /// <summary>
    ///     Create ticket summaries
    /// </summary>
    /// <param name="ticketSummaries"></param>
    /// <returns></returns>
    [HttpPost("createTicketSummaries")]
    public async Task<IActionResult> CreateTicketSummariesAsync([FromBody] List<TicketSummaryCreateDto> ticketSummaries)
    {
        return Ok(await _ticketSummaryService.CreateTicketSummariesAsync(ticketSummaries));
    }

    /// <summary>
    ///     Update ticket summaries
    /// </summary>
    /// <param name="ticketSummaries"></param>
    /// <returns></returns>
    [HttpPut("updateTicketSummaries")]
    public async Task<IActionResult> UpdateTicketSummariesAsync([FromBody] List<TicketSummaryFullDto> ticketSummaries)
    {
        return Ok(await _ticketSummaryService.UpdateTicketSummariesAsync(ticketSummaries));
    }

    /// <summary>
    ///     Delete ticket summaries by ticket id
    /// </summary>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    [HttpDelete("deleteTicketSummaries")]
    public async Task<IActionResult> DeleteTicketSummariesAsync([FromQuery] Guid ticketId)
    {
        await _ticketSummaryService.DeleteTicketSummariesAsync(ticketId);
        return Ok();
    }
}