using Azure.Messaging.ServiceBus;
using System.Text.Json;
using TicketsManager.BLL.Interfaces;
using TicketsManager.Common;
using TicketsManager.Common.Dtos.NotificationDtos;

namespace TicketsManager.BLL.ServiceBus;

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