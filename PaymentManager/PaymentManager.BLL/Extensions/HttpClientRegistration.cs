using Microsoft.Extensions.DependencyInjection;
using PaymentManager.Common;
using PaymentManager.Common.Constants;

namespace PaymentManager.BLL.Extensions;
public static class HttpClientRegistration
{
    public static void RegisterHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient("UserManager", client =>
            client.BaseAddress = new Uri($"http://{EnvironmentVariables.Host}:{ClientPortConstants.UserManagerPort}"));
    }
}
