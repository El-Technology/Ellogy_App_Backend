using AICommunicationService.Common;
using AICommunicationService.Common.Constants;
using Microsoft.Extensions.DependencyInjection;

namespace AICommunicationService.BLL.Extensions;
public static class HttpClientRegistration
{
    public static void RegisterHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient("AzureAiRequest", client =>
            client.DefaultRequestHeaders.Add("api-key", EnvironmentVariables.OpenAiKey));

        services.AddHttpClient("UserManager", client =>
            client.BaseAddress = new Uri($"http://{EnvironmentVariables.Host}:{ClientPortConstants.UserManagerPort}"));

        services.AddHttpClient("PaymentManager", client =>
            client.BaseAddress = new Uri($"http://{EnvironmentVariables.Host}:{ClientPortConstants.PaymentManagerPort}"));
    }
}
