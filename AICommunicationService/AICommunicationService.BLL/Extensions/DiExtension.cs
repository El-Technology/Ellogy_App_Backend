using AICommunicationService.BLL.Interfaces;
using AICommunicationService.BLL.Interfaces.HttpInterfaces;
using AICommunicationService.BLL.Services;
using AICommunicationService.BLL.Services.HttpServices;
using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;

namespace AICommunicationService.BLL.Extensions;

public static class DiExtension
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services, string blobStorageConnectionString)
    {
        return services
            .AddScoped<IAzureOpenAiRequestService, AzureOpenAiRequestService>()
            .AddScoped<IPromptService, PromptService>()
            .AddScoped<ICommunicationService, CommunicationService>()
            .AddScoped<IDocumentService, DocumentService>()
            .AddScoped<BlobServiceClient>(_ => new(blobStorageConnectionString))
            .AddScoped<IGroqAiRequestService, GroqAiRequestService>()
            .AddScoped<IUserExternalHttpService, UserExternalHttpService>()
            .AddScoped<IPaymentExternalHttpService, PaymentExternalHttpService>()
            .AddScoped<ITicketExternalHttpService, TicketExternalHttpService>();
    }

    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        return services
            .AddAutoMapper(typeof(DiExtension).Assembly);
    }
}
