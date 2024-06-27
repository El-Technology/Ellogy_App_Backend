using NotificationManager.BLL.Interfaces;
using NotificationManager.Common.Models;

namespace NotificationManager.BLL.Services;
public class NotificationFactory : INotifyService
{
    private readonly IEmailService _mailService;
    public NotificationFactory(IEmailService mailService)
    {
        _mailService = mailService;
    }

    public async Task SendNotificationAsync(
        NotificationModel notificationModel,
        CancellationToken cancellationToken)
    {
        var notifyService = GetNotifyService(notificationModel.Way);
        await notifyService.SendNotificationAsync(notificationModel, cancellationToken);
    }

    private INotifyService GetNotifyService(NotificationWayEnum notificationWay)
    {
        return notificationWay switch
        {
            NotificationWayEnum.Email => _mailService,
            _ => throw new Exception("Invalid NotificationWayEnum"),
        };
    }
}
