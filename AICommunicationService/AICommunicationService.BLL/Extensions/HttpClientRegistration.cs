using AICommunicationService.Common;
using AICommunicationService.Common.Constants;
using Microsoft.Extensions.DependencyInjection;

namespace AICommunicationService.BLL.Extensions;
public static class HttpClientRegistration
{
    public static void RegisterHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient("AzureAiRequest", async client =>
        {
            client.BaseAddress = new Uri(await EnvironmentVariables.GetOpenAIEndpoint);
            client.DefaultRequestHeaders.Add("api-key", await EnvironmentVariables.GetOpenAiKeyAsync);
            client.Timeout = TimeSpan.FromMinutes(3);
        });

        services.AddHttpClient("GroqAiRequest", async client =>
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await EnvironmentVariables.GetGroqKeyAsync}");
        });

        services.AddHttpClient("UserManager", async client =>
            client.BaseAddress = new Uri($"http://{await EnvironmentVariables.Host}:{ClientPortConstants.UserManagerPort}"));

        services.AddHttpClient("PaymentManager", async client =>
            client.BaseAddress = new Uri($"http://{await EnvironmentVariables.Host}:{ClientPortConstants.PaymentManagerPort}"));
    }
}
