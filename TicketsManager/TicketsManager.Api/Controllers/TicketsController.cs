using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TicketsManager.BLL.Dtos.TicketDtos;
using TicketsManager.BLL.Interfaces;

namespace TicketsManager.Api.Controllers
{
    /// <summary>
    /// Represents the API endpoints for managing tickets.
    /// </summary>
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

        /// <summary>
        /// Retrieves all tickets associated with the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>An <see cref="List{TicketResponseDto}"/> containing the list of tickets.</returns>
        [ProducesResponseType(typeof(List<TicketResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("tickets/{userId:guid}")]
        public async Task<IActionResult> GetAllTickets([Required] Guid userId)
        {
            var tickets = await _ticketsService.GetAllTicketsAsync(userId);
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
        public async Task<IActionResult> CreateTicket([FromBody] TicketCreateRequestDto createTicketRequest, [Required] Guid userId)
        {
            var createdTicket = await _ticketsService.CreateTicketAsync(createTicketRequest, userId);
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
            await _ticketsService.DeleteTicketAsync(id);
            return Ok();
        }

        /// <summary>
        /// Updates the details of a ticket.
        /// </summary>
        /// <param name="ticketUpdateRequest">The updated data for the ticket.</param>
        /// <returns>An <see cref="IActionResult"/> containing the updated ticket.</returns>
        [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpPut]
        public async Task<IActionResult> UpdateTicket([FromBody] TicketUpdateRequestDto ticketUpdateRequest)
        {
            var ticket = await _ticketsService.UpdateTicketAsync(ticketUpdateRequest);
            return Ok(ticket);
        }
    }
}
