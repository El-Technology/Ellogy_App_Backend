using Azure.Communication.Email;
using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using NotificationManager.BLL.Interfaces;
using NotificationManager.BLL.ServiceBus;
using NotificationManager.BLL.Services;

namespace NotificationManager.BLL.Extensions;
public static class DiExtensions
{
    public static IServiceCollection AddBusinessLayer(
        this IServiceCollection services,
        string busConnectionString,
        string emailConnectionString,
        string blobConnectionString)
    {
        return services
            .AddScoped<EmailClient>(_ => new(emailConnectionString))
            .AddScoped<BlobServiceClient>(_ => new(blobConnectionString))

            .AddScoped<INotifyService, NotificationFactory>()
            .AddScoped<IBlobService, BlobService>()
            .AddScoped<IEmailService, EmailService>()

            .AddSingleton<IServiceBusQueue, ServiceBusQueue>(_ => new(busConnectionString))
            .AddHostedService<ServiceBusListener>();
    }
}
