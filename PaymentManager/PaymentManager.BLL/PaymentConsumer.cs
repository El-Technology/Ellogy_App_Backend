using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaymentManager.BLL.Hubs;
using PaymentManager.BLL.Models;
using PaymentManager.BLL.Services;
using PaymentManager.Common.Constants;
using PaymentManager.DAL.Enums;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;
using Stripe;
using Stripe.Checkout;

namespace PaymentManager.BLL
{
    public class PaymentConsumer : StripeBaseService, IHostedService
    {
        private readonly ILogger<PaymentConsumer> _logger;
        private readonly ServiceBusClient _busClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<PaymentHub> _hubContext;
        private readonly ServiceBusProcessorOptions _processorOptions;

        public PaymentConsumer(ServiceBusClient busClient, IServiceProvider serviceProvider, IHubContext<PaymentHub> hubContext, ILogger<PaymentConsumer> logger)
        {
            _logger = logger;
            _hubContext = hubContext;
            _busClient = busClient;
            _serviceProvider = serviceProvider;

            _processorOptions = new ServiceBusProcessorOptions { PrefetchCount = 25 };
        }

        private async Task SendResultBySignalRAsync(string connectionId, string signalRMethodName, string message)
        {
            if (!PaymentHub.listOfConnections.Any(c => c.Equals(connectionId)))
                throw new Exception($"We can`t find connectionId => {connectionId}");

            await _hubContext.Clients.Client(connectionId).SendAsync(signalRMethodName, message);
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

        private async Task CreateFreeSubscriptionDataBaseRecordAsync(Stripe.Subscription subscription)
        {
            using var scope = _serviceProvider.CreateAsyncScope();
            var subscriptionRepository = scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>();

            try
            {
                await subscriptionRepository.CreateSubscriptionAsync(new()
                {
                    Id = Guid.NewGuid(),
                    EndDate = subscription.CurrentPeriodEnd,
                    IsActive = true,
                    IsCanceled = false,
                    StartDate = subscription.CurrentPeriodStart,
                    UserId = Guid.Parse(subscription.Metadata[MetadataConstants.UserId]),
                    SubscriptionStripeId = subscription.Id
                }, AccountPlan.Free);
            }
            finally
            {
                await scope.DisposeAsync();
            }
        }

        private async Task<string> ProcessOneTimePaymentsAsync(SessionCreateOptions message)
        {
            message.ExpiresAt = DateTime.Now.AddMinutes(30);
            var session = await GetSessionService().CreateAsync(message);

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
            return session.Url;
        }

        private async Task<string> ProcessFreeSubscription(SubscriptionCreateOptions message)
        {
            var createSubscription = await GetSubscriptionService().CreateAsync(message);
            await CreateFreeSubscriptionDataBaseRecordAsync(createSubscription);
            return "success";
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var queueMessageProcessor = _busClient.CreateProcessor(Constants.PaymentQueueName, _processorOptions);

            queueMessageProcessor.ProcessMessageAsync += async (messageArg) =>
            {
                var connectionId = string.Empty;
                var signalRMethodName = string.Empty;
                var resultMessage = string.Empty;

                try
                {
                    var messageQueueModel = JsonConvert.DeserializeObject<MessageQueueModel<object>>(messageArg.Message.Body.ToString());

                    if (messageQueueModel.Type.Equals("session"))
                    {
                        var message = JsonConvert.DeserializeObject<MessageQueueModel<SessionCreateOptions>>(messageArg.Message.Body.ToString()).CreateOptions;

                        connectionId = message.Metadata[MetadataConstants.ConnectionId];
                        signalRMethodName = message.Metadata[MetadataConstants.SignalRMethodName];

                        resultMessage = await ProcessOneTimePaymentsAsync(message);
                    }
                    else
                    {
                        var message = JsonConvert.DeserializeObject<MessageQueueModel<SubscriptionCreateOptions>>(messageArg.Message.Body.ToString()).CreateOptions;

                        connectionId = message.Metadata[MetadataConstants.ConnectionId];
                        signalRMethodName = message.Metadata[MetadataConstants.SignalRMethodName];

                        resultMessage = await ProcessFreeSubscription(message);
                    }

                    await SendResultBySignalRAsync(connectionId, signalRMethodName, resultMessage);
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
    }
}
