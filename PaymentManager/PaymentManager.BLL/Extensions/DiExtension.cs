using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Services;
using PaymentManager.Common;

namespace PaymentManager.BLL.Extensions
{
    public static class DiExtension
    {
        public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
        {
            return services
                .AddScoped<PaymentProducer>()
                .AddHostedService<PaymentConsumer>()
                .AddScoped<IPaymentSessionService, PaymentSessionService>()
                .AddSingleton<ServiceBusClient>(_ => new(EnvironmentVariables.AzureServiceBusConnectionString))
                .AddScoped<PaymentCustomerService>()
                .AddScoped<ProductCatalogService>()
                .AddScoped<WebhookService>();
        }
    }
}
