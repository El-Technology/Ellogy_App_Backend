using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketsManager.BLL.Dtos.ActionHistoryDtos;
using TicketsManager.BLL.Interfaces;

namespace TicketsManager.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ActionHistoryController : Controller
    {
        private readonly IActionHistoryService _actionHistoryService;
        public ActionHistoryController(IActionHistoryService actionHistoryService)
        {
            _actionHistoryService = actionHistoryService;
        }

        [HttpPost]
        [Route("createActionHistory")]
        public async Task<IActionResult> CreateActionHistory([FromBody] CreateActionHistoryDto createActionHistoryDto)
        {
            await _actionHistoryService.CreateActionHistoryAsync(createActionHistoryDto);
            return Ok(createActionHistoryDto);
        }

        [HttpPost]
        [Route("getActionHistory")]
        public async Task<IActionResult> GetActionHistory(Guid TicketId, [FromBody] SearchHistoryRequestDto searchHistoryRequestDto)
        {
            var result = await _actionHistoryService.GetActionHistoriesAsync(TicketId, searchHistoryRequestDto);
            return Ok(result);
        }
    }
}
