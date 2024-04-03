using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Interfaces.IHttpServices;
using PaymentManager.BLL.Services;
using PaymentManager.BLL.Services.HttpServices;
using PaymentManager.Common;

namespace PaymentManager.BLL.Extensions;

public static class DiExtension
{
    /// <summary>
    ///     This method adds the business layer services to the DI container
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        return services
            .AddScoped<PaymentProducer>()
            .AddHostedService<PaymentConsumer>()
            .AddScoped<IPaymentSessionService, PaymentSessionService>()
            .AddSingleton(_ => new ServiceBusClient(EnvironmentVariables.AzureServiceBusConnectionString))
            .AddScoped<IPaymentCustomerService, PaymentCustomerService>()
            .AddScoped<IProductCatalogService, ProductCatalogService>()
            .AddScoped<IWebhookService, WebhookService>()
            .AddScoped<IUserExternalHttpService, UserExternalHttpService>()
            .AddScoped<IWalletExternalService, WalletExternalService>();
    }

    /// <summary>
    ///     This method adds the mapping services to the DI container
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        return services
            .AddAutoMapper(typeof(DiExtension).Assembly);
    }
}