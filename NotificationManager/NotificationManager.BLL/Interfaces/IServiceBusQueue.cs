using Azure.Messaging.ServiceBus;
using NotificationManager.Common.Models;

namespace NotificationManager.BLL.Interfaces;

/// <summary>
/// Interface for the service bus queue
/// </summary>
public interface IServiceBusQueue
{
    /// <summary>
    /// Create a service bus processor which can be used to process messages from a queue
    /// </summary>
    /// <param name="serviceBusProcessorOptions"></param>
    /// <returns></returns>
    ServiceBusProcessor CreateProcessor(ServiceBusProcessorOptions serviceBusProcessorOptions);

    /// <summary>
    /// Receive a message from a queue
    /// </summary>
    /// <param name="queue"></param>
    /// <returns></returns>
    Task<string> ReceiveMessageAsync(string queue);

    /// <summary>
    /// Send a message to a queue
    /// </summary>
    /// <param name="notificationModel"></param>
    /// <returns></returns>
    Task SendMessageAsync(NotificationModel notificationModel);
}