using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TicketsManager.BLL.Dtos.TicketDtos;
using TicketsManager.BLL.Interfaces;

namespace TicketsManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/")]
public class TicketsController : Controller
{
    private readonly ITicketsService _ticketsService;

    public TicketsController(ITicketsService ticketsService)
    {
        _ticketsService = ticketsService;
    }

    [HttpGet]
    [Route("tickets/{userId:guid}")]
    public async Task<IActionResult> GetAllTickets([Required] Guid userId)
    {
        var tickets = await _ticketsService.GetAllTicketsAsync(userId);
        return Ok(tickets);
    }

    [HttpPost]
    [Route("{userId:guid}")]
    public async Task<IActionResult> CreateTicket([FromBody] TicketCreateRequestDto createTicketRequest, [Required] Guid userId)
    {
        var createdTicket = await _ticketsService.CreateTicketAsync(createTicketRequest, userId);
        return Ok(createdTicket);
    }

    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> DeleteTicket([Required] Guid id)
    {
        await _ticketsService.DeleteTicketAsync(id);
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTicket([FromBody] TicketUpdateRequestDto ticketUpdateRequest)
    {
        var ticket = await _ticketsService.UpdateTicketAsync(ticketUpdateRequest);
        return Ok(ticket);
    }
}
