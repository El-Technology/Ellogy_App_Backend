using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using UserManager.BLL.Interfaces;
using UserManager.BLL.Services;
using UserManager.Common;

namespace UserManager.BLL.Extensions;

public static class DiExtension
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        return services
            .AddScoped<IRegisterService, RegisterService>()
            .AddScoped<ILoginService, LoginService>()
            .AddScoped<IPasswordService, PasswordService>()
            .AddScoped<IMailService, MailService>()
            .AddRestSharpClient();
    }

    private static IServiceCollection AddRestSharpClient(this IServiceCollection services)
    {
        return services.AddScoped<IRestClient, RestClient>(_ => new(MailOptions.GetRestClientOptions()));
    }

    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        return services
            .AddAutoMapper(typeof(DiExtension).Assembly);
    }
}
