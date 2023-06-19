using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketsManager.BLL.Dtos.MessageDtos;
using TicketsManager.BLL.Interfaces;

namespace TicketsManager.Api.Controllers
{
    /// <summary>
    /// API controller for managing messages.
    /// </summary>
    [Authorize]
    [Route("api/[controller]/")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        /// <summary>
        /// Adds a new message.
        /// </summary>
        /// <param name="messageCreateRequest">The request data for creating a message.</param>
        /// <returns>The created message <see cref="MessageResponseDto"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> AddMessage([FromBody] MessageCreateRequestDto messageCreateRequest)
        {
            var message = await _messageService.CreateMessageAsync(messageCreateRequest);
            return Ok(message);
        }
    }
}
