using UserManager.Common.Models.NotificationModels;

namespace UserManager.BLL.Interfaces;

public interface INotificationQueueService
{
    /// <summary>
    ///     Send notification to queue
    /// </summary>
    /// <param name="notificationModel"></param>
    /// <returns></returns>
    public Task SendNotificationAsync(NotificationModel notificationModel);
}