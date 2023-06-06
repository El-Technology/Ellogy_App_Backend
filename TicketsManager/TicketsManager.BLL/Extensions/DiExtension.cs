using Microsoft.Extensions.DependencyInjection;
using TicketsManager.BLL.Interfaces;
using TicketsManager.BLL.Services;

namespace TicketsManager.BLL.Extensions;

public static class DiExtension
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        return services
            .AddScoped<ITicketsService, TicketsService>();
    }

    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        return services
            .AddAutoMapper(typeof(DiExtension).Assembly);
    }
}
