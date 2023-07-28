using Azure;
using Azure.Communication.Email;
using NotificationService.Helpers;
using NotificationService.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManager.Common;
using UserManager.Common.Models.NotificationModels;

namespace UserManager.BLL.Services;

public class MailService : IMailService
{
    private const string ContainerName = "templates";

    private readonly EmailClient _emailClient;
    private readonly IBlobService _blobService;

    private readonly Dictionary<NotificationTypeEnum, string> notificationTypePath = new()
    {
        { NotificationTypeEnum.ResetPassword, "email/ResetPasswordTemplate.html"}
    };

    public MailService(EmailClient emailClient, IBlobService blobService)
    {
        _emailClient = emailClient;
        _blobService = blobService;
    }

    //TODO add retries
    public async Task SendNotificationAsync(NotificationModel notificationModel)
    {
        var emailMessage = await GetEmailMessage(notificationModel);
        var emailSendOperation = await _emailClient.SendAsync(WaitUntil.Started, emailMessage);
    }

    private async Task<EmailMessage> GetEmailMessage(NotificationModel notificationModel)
    {
        var templatePath = notificationTypePath.GetValueOrDefault(notificationModel.Type);
        var template = await _blobService.GetTemplateAsync(ContainerName, templatePath);

        template = TemplateReplaceHelper.Replace(template, notificationModel.MetaData);

        var emailContent = new EmailContent(MailOptions.MessageSubject)
        {
            Html = template
        };

        return new(MailOptions.FromMail, notificationModel.Consumer, emailContent);
    }
}
