using RestSharp;
using RestSharp.Authenticators;

namespace UserManager.Common;

public static class MailOptions
{
    public static RestClientOptions GetRestClientOptions()
    {
        return new()
        {
            BaseUrl = new($"https://api.mailgun.net/v3/{EnvironmentVariables.MailgunDomain}"),
            Authenticator = new HttpBasicAuthenticator("api", EnvironmentVariables.MailgunApiKey)
        };
    }

    public const string FromMail = "Ellogy Support <andriy.ratushniak@fivesysdev.com>";
    public const string MessageSubject = "Password Reset";
    public const string TemplateName = "passwrod reset";
}
