using Microsoft.AspNetCore.Mvc;

namespace AICommunicationService.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TimeOut : ControllerBase
{
    [HttpGet("test")]
    public async Task<IActionResult> Test()
    {
        await Task.Delay(120000);
        return Ok("Test");
    }
}
