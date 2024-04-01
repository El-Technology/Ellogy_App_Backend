using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaymentManager.BLL.Hubs;
using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Models;
using PaymentManager.BLL.Services;
using PaymentManager.BLL.Services.HttpServices;
using PaymentManager.Common.Constants;
using PaymentManager.DAL.Enums;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;
using Stripe;
using Stripe.Checkout;
using Subscription = PaymentManager.DAL.Models.Subscription;

namespace PaymentManager.BLL;

/// <summary>
///     This class contains methods for consuming messages from the payment queue
/// </summary>
public class PaymentConsumer : StripeBaseService, IHostedService
{
    private readonly ServiceBusClient _busClient;
    private readonly IHubContext<PaymentHub> _hubContext;
    private readonly ILogger<PaymentConsumer> _logger;
    private readonly ServiceBusProcessorOptions _processorOptions;
    private readonly IServiceProvider _serviceProvider;

    public PaymentConsumer(ServiceBusClient busClient, IServiceProvider serviceProvider,
        IHubContext<PaymentHub> hubContext, ILogger<PaymentConsumer> logger)
    {
        _logger = logger;
        _hubContext = hubContext;
        _busClient = busClient;
        _serviceProvider = serviceProvider;
        _processorOptions = new ServiceBusProcessorOptions { PrefetchCount = 25 };
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var queueMessageProcessor = _busClient.CreateProcessor(Constants.PaymentQueueName, _processorOptions);

        queueMessageProcessor.ProcessMessageAsync += async messageArg =>
        {
            try
            {
                var messageQueueModel =
                    JsonConvert.DeserializeObject<MessageQueueModel<object>>(messageArg.Message.Body.ToString());

                string connectionId;
                string signalRMethodName;
                string resultMessage;

                switch (messageQueueModel.Type)
                {
                    case Constants.PaymentMode:
                        var sessionMessage = JsonConvert
                            .DeserializeObject<MessageQueueModel<SessionCreateOptions>>(
                                messageArg.Message.Body.ToString()).CreateOptions;

                        connectionId = sessionMessage.Metadata[MetadataConstants.ConnectionId];
                        signalRMethodName = sessionMessage.Metadata[MetadataConstants.SignalRMethodName];

                        resultMessage = await ProcessOneTimePaymentsAsync(sessionMessage);
                        break;

                    case Constants.SubscriptionMode:
                        var subscriptionMessage = JsonConvert
                            .DeserializeObject<MessageQueueModel<SubscriptionCreateOptions>>(messageArg.Message.Body
                                .ToString()).CreateOptions;

                        connectionId = subscriptionMessage.Metadata[MetadataConstants.ConnectionId];
                        signalRMethodName = subscriptionMessage.Metadata[MetadataConstants.SignalRMethodName];

                        resultMessage = await ProcessFreeSubscription(subscriptionMessage);
                        break;

                    default:
                        throw new Exception($"Message type {messageQueueModel.Type} was not found");
                }

                await SendResultBySignalRAsync(connectionId, signalRMethodName, resultMessage);
                await messageArg.CompleteMessageAsync(messageArg.Message, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                await messageArg.DeadLetterMessageAsync(messageArg.Message, ex.Message,
                    cancellationToken: cancellationToken);
            }
        };

        queueMessageProcessor.ProcessErrorAsync += messageArgs => throw new Exception(messageArgs.Exception.Message);

        await queueMessageProcessor.StartProcessingAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _busClient.DisposeAsync();
    }

    private async Task SendResultBySignalRAsync(string connectionId, string signalRMethodName, string message)
    {
        if (!PaymentHub.CheckIfConnectionIdExist(connectionId))
            return;

        await _hubContext.Clients.Client(connectionId).SendAsync(signalRMethodName, message);
    }

    private async Task CreatePaymentAsync(Payment payment)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var testRepo = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();

        await testRepo.CreatePaymentAsync(payment);
    }


    private async Task CreateFreeSubscriptionDataBaseRecordAsync(Stripe.Subscription subscription)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var subscriptionRepository = scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>();
        var productService = scope.ServiceProvider.GetRequiredService<IProductCatalogService>();
        var userExternalHttpService = scope.ServiceProvider.GetRequiredService<UserExternalHttpService>();

        var getProductId = subscription.Items.Data.FirstOrDefault()?.Plan.ProductId
                           ?? throw new Exception("Taking productId error");

        var productModel = await productService.GetProductAsync(getProductId);

        await subscriptionRepository.CreateSubscriptionAsync(new Subscription
        {
            Id = Guid.NewGuid(),
            Name = productModel.Name,
            Price = productModel.Price,
            EndDate = subscription.CurrentPeriodEnd,
            IsActive = true,
            IsCanceled = false,
            StartDate = subscription.CurrentPeriodStart,
            UserId = Guid.Parse(subscription.Metadata[MetadataConstants.UserId]),
            SubscriptionStripeId = subscription.Id
        }, AccountPlan.Free);
        await userExternalHttpService.UpdateAccountPlanAsync(Guid.Parse(subscription.Metadata[MetadataConstants.UserId]), AccountPlan.Free);
    }

    private async Task<bool> CheckIfFreeSubscriptionWasCreated(Guid userId)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var subscriptionRepository = scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>();

        return await subscriptionRepository.GetActiveSubscriptionAsync(userId) is not null;
    }

    private async Task<string> ProcessOneTimePaymentsAsync(SessionCreateOptions message)
    {
        message.ExpiresAt = DateTime.Now.AddMinutes(30);
        var session = await GetSessionService().CreateAsync(message);

        if (!session.Mode.Equals(Constants.SetupMode))
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
        return session.Url;
    }

    private async Task<string> ProcessFreeSubscription(SubscriptionCreateOptions message)
    {
        if (await CheckIfFreeSubscriptionWasCreated(Guid.Parse(message.Metadata[MetadataConstants.UserId])))
            return EventResultConstants.Error;

        var createSubscription = await GetSubscriptionService().CreateAsync(message);
        await CreateFreeSubscriptionDataBaseRecordAsync(createSubscription);

        return EventResultConstants.Success;
    }
}