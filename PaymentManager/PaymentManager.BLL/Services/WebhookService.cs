using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PaymentManager.BLL.Helpers;
using PaymentManager.BLL.Hubs;
using PaymentManager.BLL.Interfaces;
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
    private readonly IUserRepository _userRepository;

    public WebhookService(IPaymentRepository paymentRepository,
        IUserRepository userRepository,
        ISubscriptionRepository subscriptionRepository,
        ILogger<WebhookService> logger,
        IProductCatalogService productCatalogService,
        IHubContext<PaymentHub> hubContext)
    {
        _hubContext = hubContext;
        _productCatalogService = productCatalogService;
        _logger = logger;
        _paymentRepository = paymentRepository;
        _userRepository = userRepository;
        _subscriptionRepository = subscriptionRepository;
    }

    /// <inheritdoc cref="IWebhookService.OrderConfirmationAsync(Session)" />
    public async Task OrderConfirmationAsync(Session session)
    {
        var payment = await _paymentRepository.GetPaymentAsync(session.Id);

        if (payment?.Mode is null)
            return;

        if (payment is { UpdatedBallance: true, Mode: Constants.PaymentMode })
            return;

        switch (session.Mode)
        {
            case Constants.PaymentMode:
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
                break;

            default:
                throw new Exception("Session confirmation error");
        }
    }

    /// <inheritdoc cref="IWebhookService.ExpireSessionAsync(Session)" />
    public async Task ExpireSessionAsync(Session session)
    {
        var payment = await _paymentRepository.GetPaymentAsync(session.Id);

        if (payment?.Status == "expired")
            return;

        if (session.Status != "expired")
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
        if (subscription.CancelAtPeriodEnd)
        {
            var userId = Guid.Parse(subscription.Metadata[MetadataConstants.UserId]);

            await _subscriptionRepository.UpdateSubscriptionIsCanceledAsync(subscription.Id,
                subscription.CancelAtPeriodEnd);
            await SendEventResultAsync(userId, EventResultConstants.SubscriptionCanceled, EventResultConstants.Success);
        }
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

        await SetDefaultSubscriptionAsync(subscription.CustomerId, userId);
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
        if (invoice.SubscriptionId is null)
            return;

        var subscription = await GetSubscriptionService().GetAsync(invoice.SubscriptionId);
        var freeProduct = await _productCatalogService.GetProductByNameAsync(AccountPlan.Free.ToString());

        var newSubscription = await GetSubscriptionService().UpdateAsync(subscription.Id, new SubscriptionUpdateOptions
        {
            ProrationBehavior = "none",
            Items = new List<SubscriptionItemOptions>
            {
                new() { Id = subscription.Items.Data.First().Id, Deleted = true },
                new() { Price = freeProduct.PriceId }
            }
        });

        await GetInvoiceService().VoidInvoiceAsync(invoice.Id);

        var userId = Guid.Parse(subscription.Metadata[MetadataConstants.UserId]);

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

        await SendEventResultAsync(userId, EventResultConstants.PaymentSuccess, EventResultConstants.Error);

        await _paymentRepository.CreatePaymentAsync(new Payment
        {
            Id = Guid.NewGuid(),
            InvoiceId = subscription.LatestInvoiceId,
            Mode = "subscription payment",
            ProductName = $"Changed to {freeProduct}",
            UserId = userId,
            Status = "failed",
            AmountOfPoints = default,
            UpdatedBallance = false
        });
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

        await SendEventResultAsync(userId, EventResultConstants.PaymentSuccess, EventResultConstants.Success);
    }

    private async Task UpdateUserBalanceAsync(Guid userId, int amountOfPoints)
    {
        await _paymentRepository.UpdateBalanceAsync(userId, amountOfPoints);
        await _userRepository.UpdateTotalPurchasedTokensAsync(userId, amountOfPoints);
    }

    private async Task SetDefaultSubscriptionAsync(string customerId, string userId)
    {
        var product = (await GetProductService().SearchAsync(new ProductSearchOptions
        {
            Query = $"active:'true' AND name~'{AccountPlan.Free}'"
        })).Data.FirstOrDefault() ?? throw new Exception("Taking product for free subscription failed");

        var createdSubscription = await GetSubscriptionService().CreateAsync(new SubscriptionCreateOptions
        {
            Customer = customerId,
            Items = new List<SubscriptionItemOptions> { new() { Price = product.DefaultPriceId } },
            Metadata = new Dictionary<string, string>
            {
                { MetadataConstants.AccountPlan, AccountPlan.Free.ToString() },
                { MetadataConstants.UserId, userId },
                { MetadataConstants.ProductName, product.Name }
            }
        });

        await _subscriptionRepository.CreateSubscriptionAsync(new DAL.Models.Subscription
        {
            EndDate = createdSubscription.CurrentPeriodEnd,
            Id = Guid.NewGuid(),
            IsActive = true,
            IsCanceled = false,
            StartDate = createdSubscription.CurrentPeriodStart,
            SubscriptionStripeId = createdSubscription.Id,
            UserId = Guid.Parse(userId)
        }, AccountPlan.Free);
    }

    private async Task SendEventResultAsync(Guid userId, string methodName, string message)
    {
        var connections = PaymentHub.CheckIfUserIdExistAndReturnConnections(userId);
        if (!connections.Any())
            return;

        foreach (var connection in connections)
            await _hubContext.Clients.Client(connection.Key).SendAsync(methodName, message);
    }
}