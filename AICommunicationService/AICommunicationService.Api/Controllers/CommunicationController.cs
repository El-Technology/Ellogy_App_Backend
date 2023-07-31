using AICommunicationService.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AICommunicationService.Api.Controllers
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
        [Route("sendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] string message)
        {
            var response = await _communicationService.SendMessageAsync(message);
            return Ok(response);
        }

        [HttpPost]
        [Route("getDescription")]
        public async Task<IActionResult> GetDescription([FromBody] string userStories)
        {
            var description = await _communicationService.GetDescriptionAsync(userStories);
            return Ok(description);
        }

        [HttpPost]
        [Route("getDiagram")]
        public async Task<IActionResult> GetDiagram([FromBody] string userStories)
        {
            var diagram = await _communicationService.GetDiagramsAsync(userStories);
            return Ok(diagram);
        }

        [HttpPost]
        [Route("getIsRequestClear")]
        public async Task<IActionResult> GetIsRequestClear([FromBody] string history)
        {
            var isRequest = await _communicationService.GetIsRequestClearAsync(history);
            return Ok(isRequest);
        }

        [HttpPost]
        [Route("getPotentialSummary")]
        public async Task<IActionResult> GetPotentialSummary([FromBody] string description)
        {
            var potentialSummary = await _communicationService.GetPotentialSummaryAsync(description);
            return Ok(potentialSummary);
        }
    }
}
