using Microsoft.Extensions.DependencyInjection;
using UserManager.Common;
using UserManager.Common.Constants;

namespace UserManager.BLL.Extensions;
public static class HttpClientRegistration
{
    public static void RegisterHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient("NotificationManager", async client =>
            client.BaseAddress = new Uri($"http://{await EnvironmentVariables.Host}:{ClientPortConstants.NotificationManagerPort}"));
    }
}
