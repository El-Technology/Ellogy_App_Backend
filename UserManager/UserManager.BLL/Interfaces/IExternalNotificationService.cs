using UserManager.Common.Models.NotificationModels;

namespace UserManager.BLL.Interfaces;
public interface IExternalNotificationService
{
    Task SendNotificationAsync(NotificationModel notificationModel);
}