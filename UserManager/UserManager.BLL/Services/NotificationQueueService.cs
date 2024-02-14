using System.Text.Json;
using Azure.Messaging.ServiceBus;
using UserManager.BLL.Interfaces;
using UserManager.Common;
using UserManager.Common.Models.NotificationModels;

namespace UserManager.BLL.Services;

/// <summary>
///     Service for sending notification to queue
/// </summary>
public class NotificationQueueService : INotificationQueueService
{
    private readonly ServiceBusClient _busClient;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="busClient"></param>
    public NotificationQueueService(ServiceBusClient busClient)
    {
        _busClient = busClient;
    }

    /// <inheritdoc cref="INotificationQueueService.SendNotificationAsync" />
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