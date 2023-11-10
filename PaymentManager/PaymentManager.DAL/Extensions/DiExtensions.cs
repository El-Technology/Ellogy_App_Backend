using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaymentManager.Common;
using PaymentManager.DAL.Context.PaymentContext;
using PaymentManager.DAL.Context.UserContext;
using PaymentManager.DAL.Repositories;

namespace PaymentManager.DAL.Extensions
{
    public static class DiExtensions
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services)
        {
            return services
                .AddDbContext<PaymentContext>(c => c.UseNpgsql(EnvironmentVariables.ConnectionStringPayment))
                .AddDbContext<UserContext>(c => c.UseNpgsql(EnvironmentVariables.ConnectionString))
                .AddScoped<TestRepo>();
        }
    }
}
