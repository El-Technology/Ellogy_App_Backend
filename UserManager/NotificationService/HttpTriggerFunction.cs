using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using NotificationService.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManager.Common.Models.NotificationModels;

namespace NotificationService;

public class HttpTriggerFunction
{
    private readonly INotifyService _notifier;

    public HttpTriggerFunction(INotifyService notifier)
    {
        _notifier = notifier;
    }

    [FunctionName("HttpFunction")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "projectStarted")] HttpRequest req)
    {
        await _notifier.SendNotificationAsync(new NotificationModel
        {
            Type = NotificationTypeEnum.ProjectStarted,
            Way = NotificationWayEnum.Email,
            MetaData = new Dictionary<string, string>
            { { "{{{applicationLink}}}", $"https://{await EnvironmentVariables.AppCdnUrl}" } },
            Consumer = await EnvironmentVariables.ConsumerEmail
        });

        return new OkResult();
    }
}
