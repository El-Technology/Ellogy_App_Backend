using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationManager.BLL.Interfaces;
using NotificationManager.Common.Models;
using System.Text.Json;

namespace NotificationManager.BLL.ServiceBus;

/// <summary>
/// Service bus listener
/// </summary>
public class ServiceBusListener : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceBusQueue _serviceBusQueue;
    private readonly ILogger<ServiceBusListener> _logger;

    public ServiceBusListener(
        IServiceBusQueue serviceBusQueueService,
        ILogger<ServiceBusListener> logger,
        IServiceProvider serviceProvider)
    {
        _serviceBusQueue = serviceBusQueueService;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    private async Task SendEmailAsync(NotificationModel notificationModel, CancellationToken cancelationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var emailService = scope.ServiceProvider.GetRequiredService<INotifyService>();

        await emailService.SendNotificationAsync(notificationModel, cancelationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ServiceBusListener is starting.");

        var queueProcessor = _serviceBusQueue.CreateProcessor(new ServiceBusProcessorOptions());

        queueProcessor.ProcessMessageAsync += async args =>
        {
            try
            {
                var message = args.Message;

                var notificationModel = JsonSerializer.Deserialize<NotificationModel>(message.Body.ToString());

                await SendEmailAsync(notificationModel ?? new(), stoppingToken);

                await args.CompleteMessageAsync(message, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing. {Message}", ex.Message);
                await args.DeadLetterMessageAsync(args.Message, cancellationToken: stoppingToken);
            }
        };

        queueProcessor.ProcessErrorAsync += args =>
        {
            //review possible infinity loop
            _logger.LogError(args.Exception, "Unexpected error {Message}", args.Exception.Message);
            throw new Exception(args.Exception.Message);
        };

        await queueProcessor.StartProcessingAsync(stoppingToken);
    }
}
