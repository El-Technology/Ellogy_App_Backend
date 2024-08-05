using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketsManager.BLL.Dtos.TicketShareDtos;
using TicketsManager.BLL.Interfaces;
using TicketsManager.Common.Dtos;
using TicketsManager.Common.Helpers;

namespace TicketsManager.Api.Controllers;

/// <summary>
///    TicketShare controller which provides endpoints for ticket share operations
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class TicketShareController : Controller
{
    private readonly ITicketShareService _ticketShareService;

    /// <summary>
    /// TicketShareController constructor
    /// </summary>
    /// <param name="ticketShareService"></param>
    public TicketShareController(ITicketShareService ticketShareService)
    {
        _ticketShareService = ticketShareService;
    }

    /// This method retrieves the user id from the JWT token
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private Guid GetUserIdFromToken() =>
        TokenParseHelper.GetUserId(User);

    /// <summary>
    ///    Controller for creating ticket shares
    /// </summary>
    /// <param name="createTicketShareDto"></param>
    /// <returns></returns>
    [HttpPost("createTicketShare")]
    public async Task<IActionResult> CreateTicketShareAsync(
        [FromBody] CreateTicketShareDto createTicketShareDto)
    {
        await _ticketShareService.CreateTicketShareAsync(
            GetUserIdFromToken(), createTicketShareDto);

        return Ok();
    }

    /// <summary>
    ///   Controller for creating many ticket shares
    /// </summary>
    /// <param name="createManyTicketShareDto"></param>
    /// <returns></returns>
    [HttpPost("createManyTicketShares")]
    public async Task<IActionResult> CreateTicketSharesAsync(
               [FromBody] CreateManyTicketShareDto createManyTicketShareDto)
    {
        await _ticketShareService.CreateManyTicketSharesAsync(
            GetUserIdFromToken(), createManyTicketShareDto);

        return Ok();
    }

    /// <summary>
    ///   Controller for getting list of shares
    /// </summary>
    /// <param name="paginationRequestDto"></param>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    [HttpPost("getListOfShares")]
    public async Task<IActionResult> GetListOfSharesAsync(
        [FromBody] PaginationRequestDto paginationRequestDto, [FromQuery] Guid ticketId)
    {
        var ticketShares = await _ticketShareService.GetListOfSharesAsync(
            GetUserIdFromToken(), ticketId, paginationRequestDto);

        return Ok(ticketShares);
    }

    /// <summary>
    ///   Controller for updating ticket shares
    /// </summary>
    /// <param name="ticketShare"></param>
    /// <param name="ticketShareId"></param>
    /// <returns></returns>
    [HttpPut("updateTicketShare")]
    public async Task<IActionResult> UpdateTicketShareAsync(
        [FromBody] UpdateTicketShareDto ticketShare, [FromQuery] Guid ticketShareId)
    {
        await _ticketShareService.UpdateTicketShareAsync(
            GetUserIdFromToken(), ticketShareId, ticketShare);

        return Ok();
    }

    /// <summary>
    ///  Controller for deleting ticket shares
    /// </summary>
    /// <param name="ticketShareId"></param>
    /// <returns></returns>
    [HttpDelete("deleteTicketShare")]
    public async Task<IActionResult> DeleteTicketShareAsync(
               [FromQuery] Guid ticketShareId)
    {
        await _ticketShareService.DeleteTicketShareAsync(
                       GetUserIdFromToken(), ticketShareId);

        return Ok();
    }

    /// <summary>
    /// Controller for deleting many ticket shares
    /// </summary>
    /// <param name="ticketShareIds"></param>
    /// <returns></returns>
    [HttpDelete("deleteManyTicketShare")]
    public async Task<IActionResult> DeleteManyTicketShareAsync(
               [FromBody] List<Guid> ticketShareIds)
    {
        await _ticketShareService.DeleteManyTicketShareAsync(
                       GetUserIdFromToken(), ticketShareIds);

        return Ok();
    }
}
