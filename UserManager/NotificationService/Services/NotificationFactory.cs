using NotificationService.Interfaces;
using System.Threading.Tasks;
using UserManager.Common.Models.NotificationModels;

namespace NotificationService.Services
{
    public class NotificationFactory : INotifyService
    {
        private readonly IMailService _mailService;
        private readonly ISmsService _smsService;
        public NotificationFactory(ISmsService smsService, IMailService mailService)
        {
            _smsService = smsService;
            _mailService = mailService;
        }

        public async Task SendNotificationAsync(NotificationModel notificationModel)
        {
            var notifyService = GetNotifyService(notificationModel.Way);
            await notifyService.SendNotificationAsync(notificationModel);
        }

        private INotifyService GetNotifyService(NotificationWayEnum notificationWay)
        {
            return notificationWay switch
            {
                NotificationWayEnum.Email => _mailService,
                NotificationWayEnum.Sms => _smsService,
                _ => null,
            };
        }
    }
}
