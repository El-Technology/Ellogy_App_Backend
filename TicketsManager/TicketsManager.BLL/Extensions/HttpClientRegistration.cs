using Microsoft.Extensions.DependencyInjection;
using TicketsManager.Common;
using TicketsManager.Common.Constants;

namespace TicketsManager.BLL.Extensions;
public static class HttpClientRegistration
{
    public static void RegisterHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient("UserManager", async client =>
            client.BaseAddress = new Uri($"http://{await EnvironmentVariables.Host}:{ClientPortConstants.UserManagerPort}"));
    }
}
