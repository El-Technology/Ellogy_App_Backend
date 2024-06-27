using Azure.Messaging.ServiceBus;
using NotificationManager.BLL.Interfaces;
using NotificationManager.Common.Models;
using NotificationManager.Common.Options;
using System.Text.Json;

namespace NotificationManager.BLL.ServiceBus;

/// <summary>
/// Service bus queue
/// </summary>
/// <param name="busConnectionString"></param>
public class ServiceBusQueue(string busConnectionString) : IServiceBusQueue
{
    private readonly ServiceBusClient _serviceBusClient = new(busConnectionString);

    /// <inheritdoc cref="IServiceBusQueue.SendMessageAsync(NotificationModel)"/>
    public async Task SendMessageAsync(NotificationModel notificationModel)
    {
        var busSender = _serviceBusClient.CreateSender(NotificationQueueOptions.QueueName);
        var message = new ServiceBusMessage(JsonSerializer.Serialize(notificationModel))
        {
            ContentType = NotificationQueueOptions.MessageContentType,
        };

        await busSender.SendMessageAsync(message);
    }

    /// <inheritdoc cref="IServiceBusQueue.ReceiveMessageAsync(string)"/>
    public async Task<string> ReceiveMessageAsync(string queue)
    {
        var busReceiver = _serviceBusClient.CreateReceiver(queue);
        var message = await busReceiver.ReceiveMessageAsync();

        return message.Body.ToString();
    }

    /// <inheritdoc cref="IServiceBusQueue.CreateProcessor(ServiceBusProcessorOptions)"/>
    public ServiceBusProcessor CreateProcessor(
        ServiceBusProcessorOptions serviceBusProcessorOptions)
    {
        return _serviceBusClient.CreateProcessor(
            NotificationQueueOptions.QueueName, serviceBusProcessorOptions);
    }
}
