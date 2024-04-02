using Azure.Communication.Email;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Interfaces;
using NotificationService.Services;
using UserManager.Common;

[assembly: FunctionsStartup(typeof(NotificationService.Startup))]
namespace NotificationService
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<INotifyService, NotificationFactory>();
            builder.Services.AddScoped<IBlobService, BlobService>();
            builder.Services.AddScoped<IMailService, MailService>();
            builder.Services.AddScoped<EmailClient>(_ => new(EnvironmentVariables.EmailClientConnectionString));
            builder.Services.AddScoped<BlobServiceClient>(_ => new(EnvironmentVariables.BlobStorageConnectionString));
        }
    }
}
