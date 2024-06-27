using NotificationManager.BLL.Interfaces;
using NotificationManager.Common.Models;

namespace NotificationManager.BLL.Services;

/// <summary>
/// Factory for sending notifications
/// </summary>
public class NotificationFactory : INotifyService
{
    private readonly IEmailService _mailService;

    /// <summary>
    /// Constructor for the notification factory
    /// </summary>
    /// <param name="mailService"></param>
    public NotificationFactory(IEmailService mailService)
    {
        _mailService = mailService;
    }

    /// <inheritdoc cref="INotifyService.SendNotificationAsync(NotificationModel, CancellationToken)"/>
    public async Task SendNotificationAsync(
        NotificationModel notificationModel,
        CancellationToken cancellationToken)
    {
        var notifyService = GetNotifyService(notificationModel.Way);
        await notifyService.SendNotificationAsync(notificationModel, cancellationToken);
    }

    /// <summary>
    /// Returns the correct notification service based on the notification way
    /// </summary>
    /// <param name="notificationWay"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private INotifyService GetNotifyService(NotificationWayEnum notificationWay)
    {
        return notificationWay switch
        {
            NotificationWayEnum.Email => _mailService,
            _ => throw new Exception("Invalid NotificationWayEnum"),
        };
    }
}
