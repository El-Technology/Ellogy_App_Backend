using Azure;
using Azure.Communication.Email;
using NotificationService.Helpers;
using NotificationService.Interfaces;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using UserManager.Common;
using UserManager.Common.Constants;
using UserManager.Common.Models.NotificationModels;
using UserManager.Common.Options;

namespace NotificationService.Services;

public class MailService : IMailService
{
    private readonly EmailClient _emailClient;
    private readonly IBlobService _blobService;
    private readonly Dictionary<NotificationTypeEnum, string> notificationTypePath = new()
    {
        { NotificationTypeEnum.ResetPassword, "ResetPasswordTemplate.html"},
        { NotificationTypeEnum.Report, "ReportTemplate.html"},
        { NotificationTypeEnum.VerifyEmail, "VerifyEmailTemplate.html"}
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
        var template = await _blobService.GetTemplateAsync(BlobContainerConstants.TemplatesContainer, templatePath);

        template = TemplateReplaceHelper.Replace(template, notificationModel.MetaData);

        var emailContent = new EmailContent(MailOptions.MessageSubject)
        {
            Html = template
        };

        var emailMessage = new EmailMessage(EnvironmentVariables.MailFrom, notificationModel.Consumer, emailContent);

        if (notificationModel.BlobUrls is not null)
        {
            for (var i = 0; i < notificationModel.BlobUrls.Count; i++)
            {
                var fileName = notificationModel.BlobUrls[i];
                emailMessage.Attachments.Add(new EmailAttachment(
                    $"scr{i}.jpg",
                    MediaTypeNames.Image.Jpeg,
                    await _blobService.GetImageFromBlobAsync(fileName, BlobContainerConstants.ImagesContainer)));
            }
        }

        return emailMessage;
    }
}
