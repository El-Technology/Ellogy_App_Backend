using UserManager.Common.Models.NotificationModels;

namespace UserManager.BLL.Interfaces;

public interface IServiceBusQueue
{
    Task SendMessageAsync(NotificationModel notificationModel);
}