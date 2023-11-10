using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using PaymentManager.BLL.Services;

namespace PaymentManager.BLL.Extensions
{
    public static class DiExtension
    {
        public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
        {
            return services
                .AddScoped<PaymentProducer>()
                .AddHostedService<PaymentConsumer>()
                .AddScoped<PaymentService>()
                .AddSingleton<ServiceBusClient>(_=> new(Environment.GetEnvironmentVariable("AZURE_SERVICE_BUS_CONNECTION_STRING")));
        }
    }
}
