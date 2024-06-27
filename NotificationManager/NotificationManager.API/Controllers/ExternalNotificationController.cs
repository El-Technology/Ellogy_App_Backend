using Microsoft.AspNetCore.Mvc;
using NotificationManager.BLL.Interfaces;
using NotificationManager.Common;
using NotificationManager.Common.Models;

namespace NotificationManager.API.Controllers;

/// <summary>
/// Controller for sending external notifications
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ExternalNotificationController : ControllerBase
{
    private readonly IServiceBusQueue _serviceBusQueueService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="serviceBusQueueService"></param>
    public ExternalNotificationController(IServiceBusQueue serviceBusQueueService)
    {
        _serviceBusQueueService = serviceBusQueueService;
    }

    /// <summary>
    ///  Send notification endpoint (allows send email via http request)
    /// </summary>
    /// <param name="notificationModel"></param>
    /// <returns></returns>
    [HttpPost("sendNotification")]
    public async Task<IActionResult> SendNotificationAsync([FromBody] NotificationModel notificationModel)
    {
        await _serviceBusQueueService.SendMessageAsync(notificationModel);
        return Ok();
    }

    /// <summary>
    /// Send project started email
    /// </summary>
    /// <returns></returns>
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
