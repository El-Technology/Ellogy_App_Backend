using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using UserManager.BLL.Interfaces;
using UserManager.BLL.Services;

namespace UserManager.BLL.Extensions;

public static class DiExtension
{
    public static IServiceCollection AddBusinessLayer(
        this IServiceCollection services, string blobStorageConnectionString, string azureServiceBusConnectionString)
    {
        return services
            .AddScoped<IUserProfileService, UserProfileService>()
            .AddScoped<BlobServiceClient>(_ => new(blobStorageConnectionString))
            .AddScoped<IReportService, ReportService>()
            .AddScoped<IRefreshTokenService, RefreshTokenService>()
            .AddScoped<INotificationQueueService, NotificationQueueService>()
            .AddScoped<IRegisterService, RegisterService>()
            .AddScoped<ILoginService, LoginService>()
            .AddScoped<IPasswordService, PasswordService>()
            .AddScoped<IUserExternalService, UserExternalService>()
            .AddScoped<ServiceBusClient>(_ => new(azureServiceBusConnectionString));
    }

    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        return services
            .AddAutoMapper(typeof(DiExtension).Assembly);
    }
}
