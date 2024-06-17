using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketsManager.BLL.Dtos.TicketShareDtos;
using TicketsManager.BLL.Interfaces;
using TicketsManager.Common.Dtos;
using TicketsManager.Common.Helpers;
using TicketsManager.DAL.Models.TicketModels;

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
        var ticketShare = await _ticketShareService.CreateTicketShareAsync(
            GetUserIdFromToken(), createTicketShareDto);

        return Ok(ticketShare);
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
                       ticketId, paginationRequestDto);

        return Ok(ticketShares);
    }

    /// <summary>
    ///   Controller for updating ticket shares
    /// </summary>
    /// <param name="ticketShare"></param>
    /// <returns></returns>
    [HttpPut("updateTicketShare")]
    public async Task<IActionResult> UpdateTicketShareAsync(
               [FromBody] TicketShare ticketShare)
    {
        await _ticketShareService.UpdateTicketShareAsync(ticketShare);

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
}
