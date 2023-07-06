using Azure;
using Azure.Communication.Email;
using UserManager.BLL.Dtos.MailDtos;
using UserManager.BLL.Interfaces;
using UserManager.Common;
using UserManager.Common.Helpers;
using UserManager.Common.Options;

namespace UserManager.BLL.Services;

public class MailService : IMailService
{
    private readonly EmailClient _emailClient;

    private static readonly List<string> ResetPasswordDirectories = new() {AppContext.BaseDirectory, "ResetPasswordTemplate.html" };
    private const string ResetLinkPlaceholder = "{{{resetPasswordLink}}}";

    public MailService(EmailClient emailClient)
    {
        _emailClient = emailClient;
    }

    //TODO add retries
    public async Task SendPasswordResetLetterAsync(ResetPasswordLetterDto resetPasswordLetterDto)
    {
        var emailMessage = GetEmailMessage(resetPasswordLetterDto);

        var emailSendOperation = await _emailClient.SendAsync(WaitUntil.Started, emailMessage);
    }

    private static EmailMessage GetEmailMessage(ResetPasswordLetterDto resetPasswordLetterDto)
    {
        var htmlLetterContent = File.ReadAllText(PathBuilderHelper.BuildPath(ResetPasswordDirectories));
        htmlLetterContent = htmlLetterContent.Replace(ResetLinkPlaceholder, resetPasswordLetterDto.ResetPasswordUrl);

        var emailContent = new EmailContent(MailOptions.MessageSubject)
        {
            Html = htmlLetterContent
        };

        return new(MailOptions.FromMail, resetPasswordLetterDto.UserEmail, emailContent);
    }
}
