using TicketsManager.Common.Dtos.NotificationDtos;

namespace TicketsManager.Common.Helpers;
public static class NotificationHelper
{
    public static NotificationModel CreateSharingNotification(SharingNotificationDto sharingNotificationDto)
    {
        var message =
            $"You have been invited to collaborate on a request: <br>" +
            $"\"{sharingNotificationDto.TicketTitle}\" by {sharingNotificationDto.OwnerEmail}<br>" +
            $"Access given to: \"{sharingNotificationDto.AccessTo}\" component(s)<br>" +
            $"Your permission: \"{sharingNotificationDto.Permission}\"";

        return new NotificationModel
        {
            Consumer = sharingNotificationDto.ConsumerEmail,
            Type = NotificationTypeEnum.Notification,
            Way = NotificationWayEnum.Email,
            MetaData = new Dictionary<string, string>
            {
                { "{{{notificationType}}}", "Sharing notification" },
                { "{{{notificationMessage}}}", message },
                { "{{{applicationLink}}}", "https://app.ellogy.ai" }
            }
        };
    }
}
