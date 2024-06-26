using Azure;
using Azure.Communication.Email;
using Microsoft.IdentityModel.Tokens;
using NotificationManager.BLL.Interfaces;
using NotificationManager.Common;
using NotificationManager.Common.Constants;
using NotificationManager.Common.Helpers;
using NotificationManager.Common.Models;
using NotificationManager.Common.Options;
using System.Net.Mime;

namespace NotificationManager.BLL.Services;
public class EmailService : IEmailService
{
    private readonly EmailClient _emailClient;
    private readonly IBlobService _blobService;
    private readonly Dictionary<NotificationTypeEnum, string> notificationTypePath = new()
    {
        { NotificationTypeEnum.ResetPassword, "ResetPasswordTemplate.html"},
        { NotificationTypeEnum.Report, "ReportTemplate.html"},
        { NotificationTypeEnum.VerifyEmail, "VerifyEmailTemplate.html"},
        { NotificationTypeEnum.ProjectStarted, "ProjectStartedTemplate.html" },
        { NotificationTypeEnum.Notification, "NotificationTemplate.html" }
    };

    public EmailService(EmailClient emailClient, IBlobService blobService)
    {
        _emailClient = emailClient;
        _blobService = blobService;
    }

    public async Task SendNotificationAsync(NotificationModel notificationModel, CancellationToken cancellationToken)
    {
        var emailMessage = await GetEmailMessage(notificationModel, cancellationToken);
        _ = await _emailClient.SendAsync(WaitUntil.Started, emailMessage, cancellationToken);
    }

    private async Task<EmailMessage> GetEmailMessage(NotificationModel notificationModel, CancellationToken cancellationToken)
    {
        var template = ResourceHelper.GetHtmlTemplate(notificationTypePath.GetValueOrDefault(notificationModel.Type)!);

        var blobEndpoint = ConnectionStringParseHelper.GetPartOfConnectionString(
            await EnvironmentVariables.BlobStorageConnectionString,
            "BlobEndpoint");

        notificationModel.MetaData.Add("{{{blobEndpoint}}}", blobEndpoint ?? string.Empty);
        template = TemplateReplaceHelper.Replace(template, notificationModel.MetaData);

        var emailContent = new EmailContent(MailOptions.MessageSubject)
        {
            Html = template
        };

        var emailMessage = new EmailMessage(await EnvironmentVariables.MailFrom, notificationModel.Consumer, emailContent);

        if (!notificationModel.BlobUrls.IsNullOrEmpty())
        {
            for (var i = 0; i < notificationModel.BlobUrls!.Count; i++)
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