using UserManager.Common.Models.NotificationModels;

namespace UserManager.BLL.Interfaces
{
    public interface INotificationQueueService
    {
        public Task SendNotificationAsync(NotificationModel notificationModel);
    }
}
