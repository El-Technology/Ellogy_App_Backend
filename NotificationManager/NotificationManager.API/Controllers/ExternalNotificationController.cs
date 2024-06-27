using Microsoft.AspNetCore.Mvc;
using NotificationManager.BLL.Interfaces;
using NotificationManager.Common;
using NotificationManager.Common.Models;

namespace NotificationManager.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExternalNotificationController : ControllerBase
{
    private readonly IServiceBusQueue _serviceBusQueueService;
    public ExternalNotificationController(IServiceBusQueue serviceBusQueueService)
    {
        _serviceBusQueueService = serviceBusQueueService;
    }

    [HttpPost("sendNotification")]
    public async Task<IActionResult> SendNotificationAsync([FromBody] NotificationModel notificationModel)
    {
        await _serviceBusQueueService.SendMessageAsync(notificationModel);
        return Ok();
    }

    [HttpGet("projectStarted")]
    public async Task<IActionResult> SendProjectStartedEmail()
    {
        await _serviceBusQueueService.SendMessageAsync(new NotificationModel
        {
            Type = NotificationTypeEnum.ProjectStarted,
            Way = NotificationWayEnum.Email,
            MetaData = new Dictionary<string, string>
            { { "{{{applicationLink}}}", $"https://{await EnvironmentVariables.AppCdnUrl}" } },
            Consumer = await EnvironmentVariables.ConsumerEmail
        });

        return Ok();
    }
}
