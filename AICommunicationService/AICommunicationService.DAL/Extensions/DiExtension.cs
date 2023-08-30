using AICommunicationService.DAL.Interfaces;
using AICommunicationService.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AICommunicationService.DAL.Extensions
{
    public static class DiExtension
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            return services
                .AddScoped<IDapperRepository>(provider => new DapperRepository(connectionString))
                .AddScoped<IAIPromptRepository, AIPromptRepository>();
        }
    }
}
