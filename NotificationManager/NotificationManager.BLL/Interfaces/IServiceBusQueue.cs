using Azure.Messaging.ServiceBus;
using NotificationManager.Common.Models;

namespace NotificationManager.BLL.Interfaces;

public interface IServiceBusQueue
{
    ServiceBusProcessor CreateProcessor(ServiceBusProcessorOptions serviceBusProcessorOptions);
    Task<string> ReceiveMessageAsync(string queue);
    Task SendMessageAsync(NotificationModel notificationModel);
}