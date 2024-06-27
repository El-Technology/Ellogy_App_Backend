using Azure.Messaging.ServiceBus;
using System.Text.Json;
using UserManager.BLL.Interfaces;
using UserManager.Common.Models.NotificationModels;
using UserManager.Common.Options;

namespace UserManager.BLL.ServiceBus;

public class ServiceBusQueue : IServiceBusQueue
{
    private readonly ServiceBusClient _serviceBusClient;

    public ServiceBusQueue(ServiceBusClient serviceBusClient)
    {
        _serviceBusClient = serviceBusClient;
    }

    public async Task SendMessageAsync(NotificationModel notificationModel)
    {
        var busSender = _serviceBusClient.CreateSender(NotificationQueueOptions.QueueName);
        var message = new ServiceBusMessage(JsonSerializer.Serialize(notificationModel))
        {
            ContentType = NotificationQueueOptions.MessageContentType,
        };

        await busSender.SendMessageAsync(message);
    }
}