using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using UserManager.BLL.Interfaces;
using UserManager.BLL.Services;
using UserManager.Common;

namespace UserManager.BLL.Extensions;

public static class DiExtension
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        return services
            .AddScoped<IUserProfileService, UserProfileService>()
            .AddScoped<BlobServiceClient>(_ => new(EnvironmentVariables.BlobStorageConnectionString))
            .AddScoped<IReportService, ReportService>()
            .AddScoped<IRefreshTokenService, RefreshTokenService>()
            .AddScoped<INotificationQueueService, NotificationQueueService>()
            .AddScoped<IRegisterService, RegisterService>()
            .AddScoped<ILoginService, LoginService>()
            .AddScoped<IPasswordService, PasswordService>()
            .AddScoped<ServiceBusClient>(_ => new(EnvironmentVariables.AzureServiceBusConnectionString))

            .AddScoped<IUserExternalService, UserExternalService>();
    }

    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        return services
            .AddAutoMapper(typeof(DiExtension).Assembly);
    }
}
