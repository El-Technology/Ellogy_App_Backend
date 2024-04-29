using Azure.Communication.Email;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Interfaces;
using NotificationService.Services;
using System.IO;
using System.Threading.Tasks;
using UserManager.Common;
using UserManager.Common.Constants;

[assembly: FunctionsStartup(typeof(NotificationService.Startup))]
namespace NotificationService;

public class Startup : FunctionsStartup
{
    private IConfigurationRoot _functionConfig = null;

    private IConfigurationRoot FunctionConfig(string appDir) =>
        _functionConfig ??= new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(appDir, "appsettings.json"), optional: false, reloadOnChange: true)
            .Build();

    public override void Configure(IFunctionsHostBuilder builder)
    {
        FunctionConfig(builder.GetContext().ApplicationRootPath);
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
