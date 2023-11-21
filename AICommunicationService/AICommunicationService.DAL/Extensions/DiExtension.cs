using AICommunicationService.DAL.Context.AiCommunication;
using AICommunicationService.DAL.Context.Wallets;
using AICommunicationService.DAL.Interfaces;
using AICommunicationService.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AICommunicationService.DAL.Extensions
{
    public static class DiExtension
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services, string connectionString, string paymentConnectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            return services
                .AddDbContext<AICommunicationContext>(c => c.UseNpgsql(connectionString))
                .AddDbContext<WalletContext>(c => c.UseNpgsql(paymentConnectionString))
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IWalletRepository, WalletRepository>()
                .AddScoped<IAIPromptRepository, AIPromptRepository>();
        }
    }
}
