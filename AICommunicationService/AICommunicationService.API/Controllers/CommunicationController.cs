using AICommunicationService.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AICommunicationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunicationController : ControllerBase
    {
        private readonly ICommunicationService _communicationService;
        public CommunicationController(ICommunicationService communicationService)
        {
            _communicationService = communicationService;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] string message)
        {
            var response = await _communicationService.SendMessageAsync(message);
            return Ok(response);
        }
    }
}
