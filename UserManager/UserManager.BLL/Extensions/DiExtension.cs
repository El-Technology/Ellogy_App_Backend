using Microsoft.Extensions.DependencyInjection;
using SendGrid.Extensions.DependencyInjection;
using UserManager.BLL.Interfaces;
using UserManager.BLL.Services;
using UserManager.Common;

namespace UserManager.BLL.Extensions;

public static class DiExtension
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        services.AddSendGrid(s => s.ApiKey = EnvironmentVariables.SendGridApiKey);

        return services
            .AddScoped<IRegisterService, RegisterService>()
            .AddScoped<ILoginService, LoginService>()
            .AddScoped<IPasswordService, PasswordService>()
            .AddScoped<IMailService, MailService>();
    }

    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        return services
            .AddAutoMapper(typeof(DiExtension).Assembly);
    }
}
