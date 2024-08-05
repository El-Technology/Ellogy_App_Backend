using TicketsManager.Common.Dtos.NotificationDtos;

namespace TicketsManager.BLL.Interfaces;
public interface IServiceBusQueue
{
    Task SendMessageAsync(NotificationModel notificationModel);
}