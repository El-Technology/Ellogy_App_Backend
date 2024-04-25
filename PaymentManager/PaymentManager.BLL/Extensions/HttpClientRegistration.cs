using Microsoft.Extensions.DependencyInjection;
using PaymentManager.Common.Constants;

namespace PaymentManager.BLL.Extensions;
public static class HttpClientRegistration
{
    public static void RegisterHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient("UserManager", async client =>
            client.BaseAddress = new Uri($"http://{await EnvironmentVariables.Host}:{ClientPortConstants.UserManagerPort}"));
    }
}
