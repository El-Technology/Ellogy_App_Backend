using Microsoft.AspNetCore.Mvc;
using NotificationManager.BLL.Interfaces;
using NotificationManager.Common.Models;

namespace NotificationManager.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly IServiceBusQueue _serviceBusQueueService;
    public TestController(IServiceBusQueue serviceBusQueueService)
    {
        _serviceBusQueueService = serviceBusQueueService;
    }

    [HttpPost]
    public IActionResult Get([FromBody] NotificationModel notificationModel)
    {
        //_serviceBusQueueService.SendMessageAsync(notificationModel);
        return Ok();
    }
}
