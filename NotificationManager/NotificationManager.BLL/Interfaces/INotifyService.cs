using NotificationManager.Common.Models;

namespace NotificationManager.BLL.Interfaces;

/// <summary>
/// Interface for sending notifications using various communication channels.
/// </summary>
public interface INotifyService
{
    /// <summary>
    /// Sends a notification asynchronously using the provided <paramref name="notificationModel"/>.
    /// </summary>
    /// <param name="notificationModel">The model containing the information for the notification.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task SendNotificationAsync(NotificationModel notificationModel, CancellationToken cancellationToken);
}