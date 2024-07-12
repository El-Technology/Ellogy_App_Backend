using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketsManager.BLL.Dtos.TicketSummaryDtos;
using TicketsManager.BLL.Interfaces;
using TicketsManager.Common.Dtos;
using TicketsManager.Common.Helpers;

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
    /// This method retrieves the user id from the JWT token
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private Guid GetUserIdFromToken() =>
        TokenParseHelper.GetUserId(User);

    /// <summary>
    ///     Get all ticket summaries by ticket id
    /// </summary>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("getTicketSummariesByTicketId")]
    public async Task<IActionResult> GetTicketSummariesByTicketIdAsync([FromQuery] Guid ticketId)
    {
        var ticketSummaries = await _ticketSummaryService
            .GetTicketSummariesByTicketIdAsync(GetUserIdFromToken(), ticketId);

        return Ok(ticketSummaries);
    }

    /// <summary>
    ///     Get all ticket summaries by ticket id
    /// </summary>
    /// <param name="ticketId"></param>
    /// <param name="paginationRequestDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("getTicketSummariesByTicketId")]
    public async Task<IActionResult> GetTicketSummariesByTicketIdAsync(
        [FromQuery] Guid ticketId, [FromBody] PaginationRequestDto paginationRequestDto)
    {
        var ticketSummaries = await _ticketSummaryService
            .GetTicketSummariesByTicketIdAsync(GetUserIdFromToken(), ticketId, paginationRequestDto);

        return Ok(ticketSummaries);
    }

    /// <summary>
    ///     Create ticket summaries
    /// </summary>
    /// <param name="ticketSummaries"></param>
    /// <returns></returns>
    [HttpPost("createTicketSummaries")]
    public async Task<IActionResult> CreateTicketSummariesAsync(
        [FromBody] List<TicketSummaryCreateDto> ticketSummaries)
    {
        return Ok(await _ticketSummaryService.CreateTicketSummariesAsync(
            GetUserIdFromToken(), ticketSummaries));
    }

    /// <summary>
    ///     Update ticket summaries
    /// </summary>
    /// <param name="ticketSummaries"></param>
    /// <returns></returns>
    [HttpPut("updateTicketSummaries")]
    public async Task<IActionResult> UpdateTicketSummariesAsync(
        [FromBody] List<TicketSummaryFullDto> ticketSummaries)
    {
        return Ok(await _ticketSummaryService.UpdateTicketSummariesAsync(
            GetUserIdFromToken(), ticketSummaries));
    }

    /// <summary>
    ///     Delete ticket summaries by ticket id
    /// </summary>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    [HttpDelete("deleteTicketSummaries")]
    public async Task<IActionResult> DeleteTicketSummariesAsync([FromQuery] Guid ticketId)
    {
        await _ticketSummaryService.DeleteTicketSummariesAsync(
            GetUserIdFromToken(), ticketId);

        return Ok();
    }

    /// <summary>
    ///    Delete ticket summaries by ids
    /// </summary>
    /// <param name="summaryIds"></param>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    [HttpDelete("deleteTicketSummariesByIds")]
    public async Task<IActionResult> DeleteTicketSummariesByIdsAsync(
        [FromBody] List<Guid> summaryIds, [FromQuery] Guid ticketId)
    {
        await _ticketSummaryService.DeleteTicketSummariesByIdsAsync(
            GetUserIdFromToken(), ticketId, summaryIds);

        return Ok();
    }

    /// <summary>
    ///   Delete ticket summary acceptance criteria
    /// </summary>
    /// <param name="summaryScenarioIds"></param>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    [HttpDelete("deleteTicketSummaryScenarios")]
    public async Task<IActionResult> DeleteTicketSummaryScenariosAsync(
        [FromBody] List<Guid> summaryScenarioIds, [FromQuery] Guid ticketId)
    {
        await _ticketSummaryService.DeleteTicketSummaryScenariosAsync(
            GetUserIdFromToken(), ticketId, summaryScenarioIds);

        return Ok();
    }

    /// <summary>
    ///    Delete ticket summary acceptance criteria
    /// </summary>
    /// <param name="summaryAcceptanceCriteriaIds"></param>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    [HttpDelete("deleteTicketSummaryAcceptanceCriteria")]
    public async Task<IActionResult> DeleteTicketSummaryAcceptanceCriteriaAsync(
        [FromBody] List<Guid> summaryAcceptanceCriteriaIds, [FromQuery] Guid ticketId)
    {
        await _ticketSummaryService.DeleteTicketSummaryAcceptanceCriteriaAsync(
            GetUserIdFromToken(), ticketId, summaryAcceptanceCriteriaIds);

        return Ok();
    }
}