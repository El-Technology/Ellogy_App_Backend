using Microsoft.Extensions.DependencyInjection;
using TicketsManager.BLL.Interfaces;
using TicketsManager.BLL.Services;

namespace TicketsManager.BLL.Extensions;

public static class DiExtension
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        return services
            .AddScoped<IActionHistoryService, ActionHistoryService>()
            .AddScoped<IUsecasesService, UsecasesService>()
            .AddScoped<ITicketsService, TicketsService>()
            .AddScoped<ITicketSummaryService, TicketSummaryService>()
            .AddScoped<IUserStoryTestService, UserStoryTestService>()
            .AddScoped<ITicketShareService, TicketShareService>();
    }

    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        return services
            .AddAutoMapper(typeof(DiExtension).Assembly);
    }
}