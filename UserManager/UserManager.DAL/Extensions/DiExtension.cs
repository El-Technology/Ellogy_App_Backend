using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserManager.DAL.Context.PaymentContext;
using UserManager.DAL.Context.UserContext;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Repositories;

namespace UserManager.DAL.Extensions;

public static class DiExtension
{
    public static IServiceCollection AddDataLayer(this IServiceCollection services, string connectionString, string paymentConnectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString));

        return services
            .AddDbContext<PaymentContext>(c => c.UseNpgsql(paymentConnectionString))
            .AddDbContext<UserManagerDbContext>(c => c.UseNpgsql(connectionString))
            .AddScoped<IPaymentRepository, PaymentRepository>()
            .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IForgotPasswordRepository, ForgotPasswordRepository>();
    }
}
