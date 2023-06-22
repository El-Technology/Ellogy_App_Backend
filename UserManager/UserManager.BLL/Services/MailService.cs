using RestSharp;
using System.Text.Json;
using UserManager.BLL.Dtos.MailDtos;
using UserManager.BLL.Interfaces;
using UserManager.Common;

namespace UserManager.BLL.Services;

public class MailService : IMailService
{
    private readonly IRestClient _restClient;

    public MailService(IRestClient restClient)
    {
        _restClient = restClient;
    }

    //TODO add retries
    public async Task SendPasswordResetLetterAsync(ResetPasswordLetterDto resetPasswordLetterDto)
    {
        var request = GetRestRequest(resetPasswordLetterDto);

        await _restClient.ExecuteAsync(request);
        _restClient.Dispose();
    }

    //TODO rewrite and make builder or like this
    private static RestRequest GetRestRequest(ResetPasswordLetterDto resetPasswordLetterDto)
    {
        var request = new RestRequest("/messages", Method.Post);
        request.AddParameter("domain", EnvironmentVariables.MailgunDomain, ParameterType.UrlSegment);
        request.AddParameter("from", MailOptions.FromMail);
        request.AddParameter("to", $"{resetPasswordLetterDto.UserName} <{resetPasswordLetterDto.UserEmail}>");
        request.AddParameter("subject", MailOptions.MessageSubject);
        request.AddParameter("template", MailOptions.TemplateName);

        var templateDataObject = new
        {
            resetPasswordLink = resetPasswordLetterDto.ResetPasswordUrl
        };
        var templateDataString = JsonSerializer.Serialize(templateDataObject);
        request.AddParameter("h:X-Mailgun-Variables", templateDataString);

        return request;
    }
}
