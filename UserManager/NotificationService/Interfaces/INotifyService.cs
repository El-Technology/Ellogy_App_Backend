using System.Threading.Tasks;
using UserManager.Common.Models.NotificationModels;

namespace NotificationService.Interfaces
{
    /// <summary>
    /// Interface for sending notifications using various communication channels.
    /// </summary>
    public interface INotifyService
    {
        /// <summary>
        /// Sends a notification asynchronously using the provided <paramref name="notificationModel"/>.
        /// </summary>
        /// <param name="notificationModel">The model containing the information for the notification.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task SendNotificationAsync(NotificationModel notificationModel);
    }
}
