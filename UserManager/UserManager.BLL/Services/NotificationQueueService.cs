using Azure.Messaging.ServiceBus;
using System.Text.Json;
using UserManager.BLL.Interfaces;
using UserManager.Common;
using UserManager.Common.Models.NotificationModels;

namespace UserManager.BLL.Services
{
    public class NotificationQueueService : INotificationQueueService
    {
        private readonly ServiceBusClient _busClient;

        public NotificationQueueService(ServiceBusClient busClient)
        {
            _busClient = busClient;
        }

        public async Task SendNotificationAsync(NotificationModel notificationModel)
        {
            var busSender = _busClient.CreateSender(NotificationQueueOptions.QueueName);
            var message = new ServiceBusMessage(JsonSerializer.Serialize(notificationModel))
            {
                ContentType = NotificationQueueOptions.MessageContentType
            };

            await busSender.SendMessageAsync(message);
        }
    }
}
