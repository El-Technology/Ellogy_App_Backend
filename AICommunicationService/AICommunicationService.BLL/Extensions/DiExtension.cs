using AICommunicationService.BLL.Interfaces;
using AICommunicationService.BLL.Services;
using Microsoft.Extensions.DependencyInjection;
using OpenAI_API;
using TicketsManager.Common;

namespace AICommunicationService.BLL.Extensions
{
    public static class DiExtension
    {
        public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
        {
            return services
                .AddScoped<IPromptService, PromptService>()
                .AddScoped<ICommunicationService, CommunicationService>()
                .AddScoped<OpenAIAPI>(_ => new(EnvironmentVariables.OpenAiKey));
        }
    }
}
