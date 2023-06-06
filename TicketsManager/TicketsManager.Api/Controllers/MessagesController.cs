using Microsoft.AspNetCore.Mvc;
using TicketsManager.BLL.Dtos.MessageDtos;
using TicketsManager.BLL.Interfaces;

namespace TicketsManager.Api.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        public async Task<IActionResult> AddMessage([FromBody] MessageCreateRequestDto messageCreateRequest)
        {
            var message = await _messageService.CreateMessageAsync(messageCreateRequest);
            return Ok(message);
        }
    }
}
