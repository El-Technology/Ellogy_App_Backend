using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        private readonly ServiceBusClient _busClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<PaymentHub> _hubContext;

        public PaymentConsumer(ServiceBusClient busClient, IServiceProvider serviceProvider, IHubContext<PaymentHub> hubContext)
        {
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

                    var session = service.Create(message);

                    await CreatePaymentAsync(new Payment
                    {
                        Id = Guid.NewGuid(),
                        PaymentId = session.PaymentIntentId,
                        AmountOfPoints = int.Parse(session.Metadata[MetadataConstants.AmountOfPoint]),
                        Status = "created",
                        UserEmail = session.CustomerEmail,
                        SessionId = session.Id,
                        UpdatedBallance = false,
                        UserId = Guid.Parse(session.Metadata[MetadataConstants.UserId]),
                    });

                    var connectionId = session.Metadata[MetadataConstants.ConnectionId];
                    var signalRMethodName = session.Metadata[MetadataConstants.SignalRMethodName];

                    if (!PaymentHub.listOfConnections.Any(c => c.Equals(connectionId)))
                        throw new Exception($"We can`t find connectionId => {connectionId}");

                    await _hubContext.Clients.Client(connectionId).SendAsync(signalRMethodName, session.Url);

                    await messageArg.CompleteMessageAsync(messageArg.Message);
                }
                catch (Exception ex)
                {
                    await messageArg.DeadLetterMessageAsync(messageArg.Message);
                    throw new Exception(ex.Message);
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
