using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NotificationService.Interfaces;
using System;
using System.Threading.Tasks;
using UserManager.Common.Models.NotificationModels;

namespace NotificationService
{
    public class SendNotificationFunction
    {
        private readonly INotifyService _notifier;

        public SendNotificationFunction(INotifyService notifier)
        {
            _notifier = notifier;
        }

        [FunctionName("SendNotificationFunction")]
        public async Task Run([ServiceBusTrigger("notificationqueue", Connection = "NOTIFICATION_QUEUE_CONNECTION_STRING")] string myQueueItem, ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");
                var data = JsonConvert.DeserializeObject<NotificationModel>(myQueueItem);
                log.LogInformation($"Notification: {data}");

                await _notifier.SendNotificationAsync(data);
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.ToString());
            }
        }
    }
}
