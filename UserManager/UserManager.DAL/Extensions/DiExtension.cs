using Microsoft.Extensions.DependencyInjection;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Repositories;

namespace UserManager.DAL.Extensions;

public static class DiExtension
{
    public static IServiceCollection AddDataLayer(this IServiceCollection services, string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString));

        return services
            .AddScoped<IDapperRepository>(provider => new DapperRepository(connectionString))
            .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IForgotPasswordRepository, ForgotPasswordRepository>();
    }
}
