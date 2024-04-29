using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using NotificationService.Interfaces;
using System;
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
}
