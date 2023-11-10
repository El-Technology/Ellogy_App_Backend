using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PaymentManager.BLL.Hubs;
using PaymentManager.Common.Constants;
using PaymentManager.DAL.Models;
using PaymentManager.DAL.Repositories;
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
                MaxConcurrentCalls = 25,
                PrefetchCount = 25
            };

            var queueMessageProcessor = _busClient.CreateProcessor(Constants.PaymentRequestQueueName, processorOptions);

            queueMessageProcessor.ProcessMessageAsync += async (messageArg) =>
            {
                try
                {
                    var message = JsonConvert.DeserializeObject<SessionCreateOptions>(messageArg.Message.Body.ToString());
                    if (!IsValidEmail(message.CustomerEmail))
                    {
                        await messageArg.DeadLetterMessageAsync(messageArg.Message);
                        throw new Exception($"Wrong email = {message.CustomerEmail}");
                    }

                    Session session = service.Create(message);

                    await CreatePaymentAsync(new Payment
                    {
                        Id = Guid.NewGuid(),
                        PaymentId = session.PaymentIntentId,
                        ProductId = Guid.Parse(session.Metadata[MetadataConstants.ProductId]),
                        Status = "created",
                        UserEmail = session.CustomerEmail,
                        SessionId = session.Id,
                        UpdatedBallance = false,
                        UserId = Guid.Parse(session.Metadata[MetadataConstants.UserId])
                    });

                    var connectionId = session.Metadata[MetadataConstants.ConnectionId];
                    var signalRMethodName = session.Metadata[MetadataConstants.SignalRMethodName];

                    if (!PaymentHub.listOfConnections.Any(c => c.Equals(connectionId)))
                        throw new Exception($"We can`t find connectionId => {connectionId}");

                    await _hubContext.Clients.Client(connectionId).SendAsync(signalRMethodName, session.Url);

                    await messageArg.CompleteMessageAsync(messageArg.Message);
                }
                catch
                {
                    await messageArg.DeadLetterMessageAsync(messageArg.Message);
                    throw new Exception($"Something was wrong, try again later");
                }
            };

            queueMessageProcessor.ProcessErrorAsync += async (messageArgs) =>
            {
                Console.WriteLine(messageArgs.Exception.Message);
            };

            await queueMessageProcessor.StartProcessingAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var address = new System.Net.Mail.MailAddress(email);
                return address.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        private async Task CreatePaymentAsync(Payment payment)
        {
            using var scope = _serviceProvider.CreateAsyncScope();
            var testRepo = scope.ServiceProvider.GetRequiredService<PaymentRepository>();

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
