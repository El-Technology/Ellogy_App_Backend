using Microsoft.Extensions.DependencyInjection;
using UserManager.BLL.Interfaces;
using UserManager.BLL.Services;

namespace UserManager.BLL.Extensions;

public static class DiExtension
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        return services
            .AddScoped<IRegisterService, RegisterService>();
    }

    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        return services
            .AddAutoMapper(typeof(DiExtension).Assembly);
    }
}
