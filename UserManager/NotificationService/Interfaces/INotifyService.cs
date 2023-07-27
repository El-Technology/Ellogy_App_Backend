using System.Threading.Tasks;
using UserManager.Common.Models.NotificationModels;

namespace NotificationService.Interfaces
{
    public interface INotifyService
    {
        public Task SendNotificationAsync(NotificationModel notificationModel);
    }
}
