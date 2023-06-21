using SendGrid;
using SendGrid.Helpers.Mail;
using UserManager.BLL.Interfaces;

namespace UserManager.BLL.Services;

public class MailService : IMailService
{
    private readonly ISendGridClient _sendGridClient;

    private const string SenderEmail = "Andriy.Ratushniak@fivesysdev.com";

    public MailService(ISendGridClient sendGridClient)
    {
        _sendGridClient = sendGridClient;
    }

    public async Task SendPasswordResetLetterAsync(string resetPasswordUrl, string receiverEmail, string name)
    {
        var from = new EmailAddress(SenderEmail, "Ellogy");
        var to = new EmailAddress(receiverEmail, name);

        var templateId = "d-58b84d7175164058a11ce56952ee889e";
        var dynamicTemplateData = new
        {
            passwordResetUrl = resetPasswordUrl
        };
        var msg = MailHelper.CreateSingleTemplateEmail(from, to, templateId, dynamicTemplateData);

        var response = await _sendGridClient.SendEmailAsync(msg);
        Console.WriteLine(await response.Body.ReadAsStringAsync());
    }
}
