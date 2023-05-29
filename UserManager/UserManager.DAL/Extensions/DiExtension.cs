using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserManager.DAL.Context;

namespace UserManager.DAL.Extensions;

public static class DiExtension
{
    public static IServiceCollection AddDataLayer(this IServiceCollection services, string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString));

        return services.AddDbContext<UserManagerDbContext>(c => c.UseNpgsql(connectionString));
    }
}
