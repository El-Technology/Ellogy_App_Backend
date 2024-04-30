using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TicketsManager.BLL.Dtos.TicketDtos;
using TicketsManager.BLL.Interfaces;
using TicketsManager.Common.Dtos;
using TicketsManager.Common.Helpers;

namespace TicketsManager.Api.Controllers;

/// <summary>
/// Represents the API endpoints for managing tickets.
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]/")]
public class TicketsController : Controller
{
    private readonly ITicketsService _ticketsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TicketsController"/> class.
    /// </summary>
    /// <param name="ticketsService"></param>
    public TicketsController(ITicketsService ticketsService)
    {
        _ticketsService = ticketsService;
    }

    /// <summary>
    /// This method retrieves the user id from the JWT token
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private Guid GetUserIdFromToken() =>
        TokenParseHelper.GetUserId(User);

    /// <summary>
    /// Retrieves all tickets associated with the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="paginateRequest">Data for getting paginating list of output items.</param>
    /// <returns>An <see cref="PaginationResponseDto{TicketResponseDto}"/> containing the list of tickets.</returns>
    [ProducesResponseType(typeof(List<TicketResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [HttpPost]
    [Route("tickets/{userId:guid}")]
    public async Task<IActionResult> GetTickets(
        [Required] Guid userId, [FromBody] PaginationRequestDto paginateRequest)
    {
        var tickets = await _ticketsService.GetTicketsAsync(
            userId, paginateRequest, GetUserIdFromToken());

        return Ok(tickets);
    }

    /// <summary>
    /// Get all tickets by user and search criteria, which checks if tickets title contains some string
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="searchRequest">Data <see cref="SearchTicketsRequestDto"/> for searching tickets, also contains pagination model.</param>
    /// <returns>An <see cref="PaginationResponseDto{TicketResponseDto}"/> containing the list of tickets.</returns>
    [ProducesResponseType(typeof(List<TicketResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [HttpPost]
    [Route("search/{userId:guid}")]
    public async Task<IActionResult> SearchTickets(
        [Required] Guid userId, [FromBody] SearchTicketsRequestDto searchRequest)
    {
        var tickets = await _ticketsService.SearchTicketsByNameAsync(
            userId, searchRequest, GetUserIdFromToken());

        return Ok(tickets);
    }

    /// <summary>
    /// Creates a new ticket for the specified user.
    /// </summary>
    /// <param name="createTicketRequest">The data required to create a ticket.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>An <see cref="TicketResponseDto"/> containing the created ticket.</returns>
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [HttpPost]
    [Route("{userId:guid}")]
    public async Task<IActionResult> CreateTicket(
        [Required] Guid userId, [FromBody] TicketCreateRequestDto createTicketRequest)
    {
        var createdTicket = await _ticketsService.CreateTicketAsync(
            createTicketRequest, userId, GetUserIdFromToken());

        return Ok(createdTicket);
    }

    /// <summary>
    /// Deletes the ticket with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the ticket.</param>
    /// <returns>An <see cref="StatusCodes.Status200OK"/> indicating the success of the deletion operation.</returns>
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> DeleteTicket([Required] Guid id)
    {
        await _ticketsService.DeleteTicketAsync(id, GetUserIdFromToken());
        return Ok();
    }

    /// <summary>
    /// Updates the details of a ticket.
    /// </summary>
    /// <param name="ticketUpdateRequest">The updated data for the ticket.</param>
    /// <param name="ticketId">The id of the user, to which belongs that ticket.</param>
    /// <returns>An <see cref="IActionResult"/> containing the updated ticket.</returns>
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [HttpPut("{ticketId:guid}")]
    public async Task<IActionResult> UpdateTicket(
        Guid ticketId, [FromBody] TicketUpdateRequestDto ticketUpdateRequest)
    {
        var ticket = await _ticketsService.UpdateTicketAsync(
            ticketId, ticketUpdateRequest, GetUserIdFromToken());

        return Ok(ticket);
    }

    /// <summary>
    /// Generates a DOC document using the provided base64-encoded data asynchronously.
    /// </summary>
    /// <param name="base64Data">An array of base64-encoded data to be included in the DOC.</param>
    /// <returns>Returns the generated DOC file as a downloadable response.</returns>
    [HttpPost]
    [Route("generateDoc")]
    public IActionResult GenerateDoc([FromBody] string[] base64Data)
    {
        var file = _ticketsService.DownloadAsDoc(base64Data);

        return File(
            file,
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "Report.docx");
    }
}
