using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using TicketsManager.BLL.Interfaces;
using TicketsManager.BLL.Interfaces.External;
using TicketsManager.BLL.ServiceBus;
using TicketsManager.BLL.Services;
using TicketsManager.BLL.Services.External;

namespace TicketsManager.BLL.Extensions;

public static class DiExtension
{
    public static IServiceCollection AddBusinessLayer(
        this IServiceCollection services,
        string busConnectionString)
    {
        return services
            .AddScoped<IActionHistoryService, ActionHistoryService>()
            .AddScoped<IUsecasesService, UsecasesService>()
            .AddScoped<ITicketsService, TicketsService>()
            .AddScoped<ITicketSummaryService, TicketSummaryService>()
            .AddScoped<IUserStoryTestService, UserStoryTestService>()
            .AddScoped<ITicketShareService, TicketShareService>()
            .AddScoped<IUserExternalHttpService, UserExternalHttpService>()
            .AddScoped<IServiceBusQueue, ServiceBusQueue>()
            .AddScoped<ITicketShareExternalService, TicketShareExternalService>()
            .AddScoped<ServiceBusClient>(_ => new(busConnectionString))
            .AddScoped<ITicketMessageService, TicketMessageService>();
    }

    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        return services
            .AddAutoMapper(typeof(DiExtension).Assembly);
    }
}