using Azure.Communication.Email;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Interfaces;
using NotificationService.Services;
using System.Threading.Tasks;
using UserManager.BLL.Services;
using UserManager.Common;
using UserManager.Common.Constants;

[assembly: FunctionsStartup(typeof(NotificationService.Startup))]
namespace NotificationService;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        ConfigureServicesAsync(builder).GetAwaiter().GetResult();
    }

    private async Task ConfigureServicesAsync(IFunctionsHostBuilder builder)
    {
        builder.Services.AddScoped<INotifyService, NotificationFactory>();
        builder.Services.AddScoped<IBlobService, BlobService>();
        builder.Services.AddScoped<IMailService, MailService>();

        var emailClientConnectionString = await EnvironmentVariables.GetSecretAsync(SecretNames.EmailClientConnectionString);
        var blobStorageConnectionString = await EnvironmentVariables.GetSecretAsync(SecretNames.BlobStorageConnectionString);

        builder.Services.AddScoped<EmailClient>(_ => new(emailClientConnectionString));
        builder.Services.AddScoped<BlobServiceClient>(_ => new(blobStorageConnectionString));
    }
}
