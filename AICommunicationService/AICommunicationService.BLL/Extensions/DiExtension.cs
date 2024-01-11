using AICommunicationService.BLL.Interfaces;
using AICommunicationService.BLL.Services;
using Microsoft.Extensions.DependencyInjection;
using AICommunicationService.Common;
using Azure.Storage.Blobs;

namespace AICommunicationService.BLL.Extensions
{
    public static class DiExtension
    {
        public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
        {
            return services
                .AddScoped<IAzureOpenAiRequestService, AzureOpenAiRequestService>()
                .AddScoped<IPromptService, PromptService>()
                .AddScoped<ICommunicationService, CommunicationService>()
                .AddScoped<IDocumentService, DocumentService>()
                .AddScoped<BlobServiceClient>(_ => new(EnvironmentVariables.BlobStorageConnectionString)); ;
        }
    }
}
