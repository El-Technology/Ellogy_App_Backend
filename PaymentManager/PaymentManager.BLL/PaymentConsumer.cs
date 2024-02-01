using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaymentManager.BLL.Hubs;
using PaymentManager.Common.Constants;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;
using Stripe.Checkout;

namespace PaymentManager.BLL
{
    public class PaymentConsumer : IHostedService
    {
        private readonly ILogger<PaymentConsumer> _logger;
        private readonly ServiceBusClient _busClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<PaymentHub> _hubContext;

        public PaymentConsumer(ServiceBusClient busClient, IServiceProvider serviceProvider, IHubContext<PaymentHub> hubContext, ILogger<PaymentConsumer> logger)
        {
            _logger = logger;
            _hubContext = hubContext;
            _busClient = busClient;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var service = new SessionService();

            var processorOptions = new ServiceBusProcessorOptions
            {
                PrefetchCount = 25
            };

            var queueMessageProcessor = _busClient.CreateProcessor(Constants.PaymentQueueName, processorOptions);

            queueMessageProcessor.ProcessMessageAsync += async (messageArg) =>
            {
                try
                {
                    var message = JsonConvert.DeserializeObject<SessionCreateOptions>(messageArg.Message.Body.ToString());
                    message.ExpiresAt = DateTime.Now.AddMinutes(30);
                    var session = await service.CreateAsync(message);

                    if (!session.Mode.Equals(Constants.SETUP_MODE))
                    {
                        await CreatePaymentAsync(new Payment
                        {
                            Id = Guid.NewGuid(),
                            PaymentId = session.PaymentIntentId,
                            InvoiceId = session.InvoiceId,
                            AmountOfPoints = int.Parse(session.Metadata[MetadataConstants.AmountOfPoint]),
                            Status = "created",
                            UserEmail = session.CustomerEmail ?? string.Empty,
                            SessionId = session.Id,
                            UpdatedBallance = false,
                            UserId = Guid.Parse(session.Metadata[MetadataConstants.UserId]),
                            Mode = session.Mode,
                            ProductName = session.Metadata[MetadataConstants.ProductName]
                        });
                    }

                    var connectionId = session.Metadata[MetadataConstants.ConnectionId];
                    var signalRMethodName = session.Metadata[MetadataConstants.SignalRMethodName];

                    if (!PaymentHub.listOfConnections.Any(c => c.Equals(connectionId)))
                        throw new Exception($"We can`t find connectionId => {connectionId}");

                    await _hubContext.Clients.Client(connectionId).SendAsync(signalRMethodName, session.Url);

                    await messageArg.CompleteMessageAsync(messageArg.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    await messageArg.DeadLetterMessageAsync(messageArg.Message);
                }
            };

            queueMessageProcessor.ProcessErrorAsync += (messageArgs) =>
            {
                throw new Exception(messageArgs.Exception.Message);
            };

            await queueMessageProcessor.StartProcessingAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _busClient.DisposeAsync();
        }

        private async Task CreatePaymentAsync(Payment payment)
        {
            using var scope = _serviceProvider.CreateAsyncScope();
            var testRepo = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();

            try
            {
                await testRepo.CreatePaymentAsync(payment);
            }
            finally
            {
                await scope.DisposeAsync();
            }
        }
    }
}
