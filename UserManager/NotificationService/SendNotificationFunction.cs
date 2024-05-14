using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using NotificationService.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManager.Common;
using UserManager.Common.Models.NotificationModels;

namespace NotificationService;

public class SendNotificationFunction
{
    private readonly INotifyService _notifier;

    public SendNotificationFunction(INotifyService notifier)
    {
        _notifier = notifier;
    }

    [FunctionName("SendNotificationFunction")]
    public async Task Run([ServiceBusTrigger(NotificationQueueOptions.QueueName, Connection = "NOTIFICATION_QUEUE_CONNECTION_STRING")] string myQueueItem)
    {
        try
        {
            var data = JsonConvert.DeserializeObject<NotificationModel>(myQueueItem);
            await _notifier.SendNotificationAsync(data);
        }
        catch (Exception)
        {
        }
    }

    [FunctionName("HttpFunction")]
    public async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "projectStarted")] HttpRequest req)
    {
        await _notifier.SendNotificationAsync(new NotificationModel
        {
            Type = NotificationTypeEnum.VerifyEmail,
            Way = NotificationWayEnum.Email,
            MetaData = new Dictionary<string, string>
            { { "{{{firstName}}}", "sharkovskiy1@gmail.com" }, { "{{{verifyEmailAddressLink}}}", "asd" } },
            Consumer = "sharkovskiy1@gmail.com"
        });

        return new OkResult();
    }
}
