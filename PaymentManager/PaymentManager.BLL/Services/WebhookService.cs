using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PaymentManager.BLL.Helpers;
using PaymentManager.BLL.Hubs;
using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Interfaces.IHttpServices;
using PaymentManager.Common.Constants;
using PaymentManager.DAL.Enums;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;
using Stripe;
using Stripe.Checkout;
using Subscription = Stripe.Subscription;


namespace PaymentManager.BLL.Services;

public class WebhookService : StripeBaseService, IWebhookService
{
    private readonly IHubContext<PaymentHub> _hubContext;
    private readonly ILogger<WebhookService> _logger;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IProductCatalogService _productCatalogService;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserExternalHttpService _userExternalHttpService;

    public WebhookService(IPaymentRepository paymentRepository,
        ISubscriptionRepository subscriptionRepository,
        ILogger<WebhookService> logger,
        IProductCatalogService productCatalogService,
        IHubContext<PaymentHub> hubContext,
        IUserExternalHttpService userExternalHttpService)
    {
        _userExternalHttpService = userExternalHttpService;
        _hubContext = hubContext;
        _productCatalogService = productCatalogService;
        _logger = logger;
        _paymentRepository = paymentRepository;
        _subscriptionRepository = subscriptionRepository;
    }

    /// <inheritdoc cref="IWebhookService.OrderConfirmationAsync(Session)" />
    public async Task OrderConfirmationAsync(Session session)
    {
        var payment = await _paymentRepository.GetPaymentAsync(session.Id);

        ArgumentNullException.ThrowIfNull(payment, nameof(payment));
        ArgumentNullException.ThrowIfNull(payment.Mode, nameof(payment.Mode));

        if (payment.UpdatedBallance || !session.Mode.Equals(Constants.PaymentMode))
            return;

        await _paymentRepository.UpdatePaymentAsync(new Payment
        {
            PaymentId = session.PaymentIntentId,
            AmountOfPoints = int.Parse(session.Metadata[MetadataConstants.AmountOfPoint]),
            Status = session.Status,
            UserEmail = session.CustomerEmail,
            UserId = payment.UserId,
            SessionId = session.Id,
            UpdatedBallance = true
        });
        await UpdateUserBalanceAsync(payment.UserId, payment.AmountOfPoints);
    }

    /// <inheritdoc cref="IWebhookService.ExpireSessionAsync(Session)" />
    public async Task ExpireSessionAsync(Session session)
    {
        var payment = await _paymentRepository.GetPaymentAsync(session.Id);

        if (payment is not { Status: not Constants.ExpiredStatus }) return;

        if (session.Status != Constants.ExpiredStatus)
            await GetSessionService().ExpireAsync(session.Id);

        await _paymentRepository.UpdatePaymentAsync(new Payment
        {
            PaymentId = session.PaymentIntentId,
            AmountOfPoints = int.Parse(session.Metadata[MetadataConstants.AmountOfPoint]),
            Status = session.Status,
            SessionId = session.Id,
            UpdatedBallance = false
        });

        _logger.LogInformation($"{session.Id} - was expired");
    }

    /// <inheritdoc cref="IWebhookService.UpdateSubscriptionAsync(Subscription)" />
    public async Task UpdateSubscriptionAsync(Subscription subscription)
    {
        if (!subscription.CancelAtPeriodEnd) return;

        var userId = Guid.Parse(subscription.Metadata[MetadataConstants.UserId]);

        await _subscriptionRepository.UpdateSubscriptionIsCanceledAsync(subscription.Id,
            subscription.CancelAtPeriodEnd);

        await SendEventResultAsync(userId, EventResultConstants.SubscriptionCanceled, EventResultConstants.Success);

        await _subscriptionRepository.UpdateSubscriptionStatusAsync(subscription.Id,
            SubscriptionStatusEnum.PendingCancellation);
    }

    /// <inheritdoc cref="IWebhookService.DeleteSubscriptionAsync(Subscription)" />
    public async Task DeleteSubscriptionAsync(Subscription subscription)
    {
        var userId = subscription.Metadata[MetadataConstants.UserId];

        var getProductId = subscription.Items.Data.FirstOrDefault()?.Plan.ProductId
                           ?? throw new Exception("Taking productId error");

        var productModel = await _productCatalogService.GetProductAsync(getProductId);

        await _subscriptionRepository.UpdateSubscriptionAsync(new DAL.Models.Subscription
        {
            Name = productModel.Name,
            Price = productModel.Price,
            SubscriptionStripeId = subscription.Id,
            StartDate = subscription.CurrentPeriodStart,
            EndDate = subscription.CurrentPeriodEnd,
            IsActive = false,
            UserId = Guid.Parse(userId),
            IsCanceled = true
        }, null);
        await _userExternalHttpService.UpdateAccountPlanAsync(Guid.Parse(userId), null);
    }

    /// <inheritdoc cref="IWebhookService.CreateCustomerAsync(Customer)" />
    public async Task CreateCustomerAsync(Customer customer)
    {
        var userId = Guid.Parse(customer.Metadata[MetadataConstants.UserId]);
        await SendEventResultAsync(userId, EventResultConstants.CustomerCreated, EventResultConstants.Success);
    }

    /// <inheritdoc cref="IWebhookService.InvoiceFailedHandleAsync(Invoice)" />
    public async Task InvoiceFailedHandleAsync(Invoice invoice)
    {
        if (invoice.SubscriptionId is null) return;

        var subscription = await GetSubscriptionService().GetAsync(invoice.SubscriptionId);
        var userId = Guid.Parse(subscription.Metadata[MetadataConstants.UserId]);

        if (invoice.PeriodEnd < invoice.WebhooksDeliveredAt)
        {
            var freeProduct = await _productCatalogService.GetProductByNameAsync(AccountPlan.Free.ToString());
            var newSubscription = await UpdateSubscriptionPaymentFailCaseAsync(subscription, freeProduct.PriceId);

            await _subscriptionRepository.UpdateSubscriptionAsync(new DAL.Models.Subscription
            {
                Name = freeProduct.Name,
                Price = freeProduct.Price,
                SubscriptionStripeId = newSubscription.Id,
                StartDate = newSubscription.CurrentPeriodStart,
                EndDate = newSubscription.CurrentPeriodEnd,
                IsActive = true,
                UserId = userId,
                IsCanceled = newSubscription.CancelAtPeriodEnd
            }, AccountPlan.Free);
            await _userExternalHttpService.UpdateAccountPlanAsync(userId, AccountPlan.Free);
        }
        else
            await UpdateSubscriptionPaymentFailCaseAsync(subscription, invoice.Lines.Data.FirstOrDefault()?.Price.Id);

        await GetInvoiceService().VoidInvoiceAsync(invoice.Id);
        await SendEventResultAsync(userId, EventResultConstants.PaymentSuccess, EventResultConstants.Error);

        await _paymentRepository.CreatePaymentAsync(new Payment
        {
            Id = Guid.NewGuid(),
            InvoiceId = subscription.LatestInvoiceId,
            Mode = "subscription payment",
            ProductName = $"Changed",
            UserId = userId,
            Status = "failed",
            AmountOfPoints = default,
            UpdatedBallance = false
        });

        await _subscriptionRepository.UpdateSubscriptionStatusAsync(invoice.SubscriptionId,
            SubscriptionStatusEnum.PaymentFailed);
    }

    public async Task InvoiceSucceededHandleAsync(Invoice invoice)
    {
        var subscription = await GetSubscriptionService().GetAsync(invoice.SubscriptionId);
        var userId = Guid.Parse(subscription.Metadata[MetadataConstants.UserId]);

        var getProductId = subscription.Items.Data.FirstOrDefault()?.Plan.ProductId
                           ?? throw new Exception("Taking productId error");

        var productModel = await _productCatalogService.GetProductAsync(getProductId);
        var productName = productModel.Name[..productModel.Name.IndexOf("/", StringComparison.Ordinal)];
        var accountPlanEnum = Enum.Parse<AccountPlan>(productName);
        var amountOfTokens = SubscriptionHelper.GetAmountOfTokens(accountPlanEnum);

        await _subscriptionRepository.UpdateSubscriptionAsync(new DAL.Models.Subscription
        {
            Name = productModel.Name,
            Price = productModel.Price,
            SubscriptionStripeId = invoice.SubscriptionId,
            StartDate = subscription.CurrentPeriodStart,
            EndDate = subscription.CurrentPeriodEnd,
            IsActive = true,
            UserId = userId,
            IsCanceled = subscription.CancelAtPeriodEnd
        }, accountPlanEnum);
        await _userExternalHttpService.UpdateAccountPlanAsync(userId, accountPlanEnum);

        await UpdateUserBalanceAsync(userId, amountOfTokens);

        await _paymentRepository.CreatePaymentAsync(new Payment
        {
            Id = Guid.NewGuid(),
            InvoiceId = subscription.LatestInvoiceId,
            Mode = "subscription update",
            ProductName = productName,
            UserId = userId,
            Status = "updated",
            AmountOfPoints = amountOfTokens,
            UpdatedBallance = true
        });

        await _subscriptionRepository.UpdateSubscriptionStatusAsync(invoice.SubscriptionId,
            SubscriptionStatusEnum.WithoutChanges);
        await SendEventResultAsync(userId, EventResultConstants.PaymentSuccess, EventResultConstants.Success);
    }

    private async Task UpdateUserBalanceAsync(Guid userId, int amountOfPoints)
    {
        await _paymentRepository.UpdateBalanceAsync(userId, amountOfPoints);
        await _userExternalHttpService.UpdateTotalPurchasedTokensAsync(userId, amountOfPoints);
    }

    private async Task SendEventResultAsync(Guid userId, string methodName, string message)
    {
        var connections = PaymentHub.CheckIfUserIdExistAndReturnConnections(userId);
        if (!connections.Any())
            return;

        foreach (var connection in connections)
            await _hubContext.Clients.Client(connection.Key).SendAsync(methodName, message);
    }

    private async Task<Subscription> UpdateSubscriptionPaymentFailCaseAsync(Subscription subscription, string? priceId)
    {
        ArgumentNullException.ThrowIfNull(priceId, nameof(priceId));

        var updatedSubscription = await GetSubscriptionService().UpdateAsync(subscription.Id, new SubscriptionUpdateOptions
        {
            ProrationBehavior = "none",
            Items = new List<SubscriptionItemOptions>
                {
                    new() { Id = subscription.Items.Data.First().Id, Deleted = true },
                    new() { Price = priceId }
                }
        });

        return updatedSubscription;
    }
}