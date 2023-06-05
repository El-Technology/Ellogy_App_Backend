using Microsoft.Extensions.DependencyInjection;

namespace TicketsManager.BLL.Extensions;

public static class DiExtension
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        return services
            .AddAutoMapper(typeof(DiExtension).Assembly);
    }
}
