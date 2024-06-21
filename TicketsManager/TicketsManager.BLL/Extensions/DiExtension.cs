using Microsoft.Extensions.DependencyInjection;
using TicketsManager.BLL.Interfaces;
using TicketsManager.BLL.Interfaces.External;
using TicketsManager.BLL.Services;
using TicketsManager.BLL.Services.External;

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
            .AddScoped<ITicketShareService, TicketShareService>()
            .AddScoped<IUserExternalHttpService, UserExternalHttpService>()
            .AddScoped<ITicketShareExternalService, TicketShareExternalService>();
    }

    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        return services
            .AddAutoMapper(typeof(DiExtension).Assembly);
    }
}