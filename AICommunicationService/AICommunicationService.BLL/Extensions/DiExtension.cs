using AICommunicationService.BLL.Interfaces;
using AICommunicationService.BLL.Services;
using AICommunicationService.BLL.Services.HttpServices;
using AICommunicationService.Common;
using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;

namespace AICommunicationService.BLL.Extensions;

public static class DiExtension
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        return services
            .AddScoped<IAzureOpenAiRequestService, AzureOpenAiRequestService>()
            .AddScoped<IPromptService, PromptService>()
            .AddScoped<ICommunicationService, CommunicationService>()
            .AddScoped<IDocumentService, DocumentService>()
            .AddScoped<BlobServiceClient>(_ => new(EnvironmentVariables.BlobStorageConnectionString))

            .AddScoped<UserExternalHttpService>()
            .AddScoped<PaymentExternalHttpService>();
    }
    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        return services
            .AddAutoMapper(typeof(DiExtension).Assembly);
    }
}
