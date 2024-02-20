using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TicketsManager.DAL.Context;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Repositories;

namespace TicketsManager.DAL.Extensions;

public static class DiExtension
{
    public static IServiceCollection AddDataLayer(this IServiceCollection services, string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString));

        return services
            .AddDbContext<TicketsManagerDbContext>(c => c.UseNpgsql(connectionString))
            .AddScoped<IActionHistoryRepository, ActionHistoryRepository>()
            .AddScoped<ITicketsRepository, TicketsRepository>()
            .AddScoped<IUsecaseRepository, UsecaseRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<ITicketSummaryRepository, TicketSummaryRepository>();
    }
}