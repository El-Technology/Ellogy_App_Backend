using AICommunicationService.BLL.Interfaces;
using AICommunicationService.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AICommunicationService.BLL.Extensions
{
    public static class DiExtension
    {
        public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
        {
            return services
                .AddScoped<IPromptService, PromptService>()
                .AddScoped<ICommunicationService, CommunicationService>();
        }
    }
}
