using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaymentManager.DAL.Context;
using PaymentManager.DAL.Repositories;

namespace PaymentManager.DAL.Extensions
{
    public static class DiExtensions
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            return services
                .AddDbContext<PaymentContext>(c => c.UseNpgsql(connectionString))
                .AddScoped<TestRepo>();
        }
    }
}
