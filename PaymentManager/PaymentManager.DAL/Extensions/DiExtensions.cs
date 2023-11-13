using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaymentManager.Common;
using PaymentManager.DAL.Context.PaymentContext;
using PaymentManager.DAL.Context.UserContext;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Repositories;

namespace PaymentManager.DAL.Extensions
{
    public static class DiExtensions
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services, string connectionString, string paymentConnectionString)
        {
            return services
                .AddDbContext<PaymentContext>(c => c.UseNpgsql(paymentConnectionString))
                .AddDbContext<UserContext>(c => c.UseNpgsql(connectionString))
                .AddScoped<IPaymentRepository, PaymentRepository>()
                .AddScoped<IUserRepository, UserRepository>();
        }
    }
}