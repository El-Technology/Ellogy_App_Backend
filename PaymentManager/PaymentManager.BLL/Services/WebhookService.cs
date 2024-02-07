using Microsoft.Extensions.Logging;
using PaymentManager.BLL.Interfaces;
using PaymentManager.Common.Constants;
using PaymentManager.DAL.Enums;
using PaymentManager.DAL.Interfaces;
using Stripe;
using Stripe.Checkout;


namespace PaymentManager.BLL.Services
{
    public class WebhookService : StripeBaseService, IWebhookService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ILogger<WebhookService> _logger;
        private readonly IProductCatalogService _productCatalogService;

        public WebhookService(IPaymentRepository paymentRepository,
            IUserRepository userRepository,
            ISubscriptionRepository subscriptionRepository,
            ILogger<WebhookService> logger,
            IProductCatalogService productCatalogService)
        {
            _productCatalogService = productCatalogService;
            _logger = logger;
            _paymentRepository = paymentRepository;
            _userRepository = userRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        /// <inheritdoc cref="IWebhookService.OrderConfirmationAsync(Session)"/>
        public async Task OrderConfirmationAsync(Session session)
        {
            var payment = await _paymentRepository.GetPaymentAsync(session.Id);

            if (payment is null)
                return;

            if (payment.Mode is null)
                return;

            if (payment.UpdatedBallance && payment.Mode.Equals(Constants.PAYMENT_MODE))
                return;

            switch (session.Mode)
            {
                case Constants.PAYMENT_MODE:
                    await _paymentRepository.UpdatePaymentAsync(new()
                    {
                        PaymentId = session.PaymentIntentId,
                        AmountOfPoints = int.Parse(session.Metadata[MetadataConstants.AmountOfPoint]),
                        Status = session.Status,
                        UserEmail = session.CustomerEmail,
                        UserId = payment.UserId,
                        SessionId = session.Id,
                        UpdatedBallance = true,
                    });
                    await _paymentRepository.UpdateBalanceAsync(payment.UserId, payment.AmountOfPoints);
                    await _userRepository.UpdateTotalPurchasedTokensAsync(payment.UserId, payment.AmountOfPoints);
                    break;

                case Constants.SUBSCRIPTION_MODE:
                    break;

                case Constants.SETUP_MODE:
                    break;

                default:
                    throw new Exception("Session confirmation error");
            }
        }

        /// <inheritdoc cref="IWebhookService.ExpireSessionAsync(Session)"/>
        public async Task ExpireSessionAsync(Session session)
        {
            var payment = await _paymentRepository.GetPaymentAsync(session.Id);

            if (payment is null)
                return;

            if (payment.Status == "expired")
                return;

            if (session.Status != "expired")
                await GetSessionService().ExpireAsync(session.Id);

            await _paymentRepository.UpdatePaymentAsync(new()
            {
                PaymentId = session.PaymentIntentId,
                AmountOfPoints = int.Parse(session.Metadata[MetadataConstants.AmountOfPoint]),
                Status = session.Status,
                SessionId = session.Id,
                UpdatedBallance = false,
            });

            _logger.LogInformation($"{session.Id} - was expired");
        }

        /// <inheritdoc cref="IWebhookService.UpdateSubscriptionAsync(Subscription)"/>
        public async Task UpdateSubscriptionAsync(Subscription subscription)
        {
            var getProductId = subscription.Items.Data.FirstOrDefault()?.Plan.ProductId
                ?? throw new Exception("Taking productId error");

            var productModel = await _productCatalogService.GetProductAsync(getProductId);
            var productName = productModel.Name.Substring(0, productModel.Name.IndexOf("/"));

            await _paymentRepository.CreatePaymentAsync(new()
            {
                Id = Guid.NewGuid(),
                InvoiceId = subscription.LatestInvoiceId,
                Mode = "subscription update",
                ProductName = productName,
                UserId = Guid.Parse(subscription.Metadata[MetadataConstants.UserId]),
                Status = "updated",
                AmountOfPoints = 0,
                UpdatedBallance = true
            });

            if (subscription.CancelAtPeriodEnd)
                await _subscriptionRepository.UpdateSubscriptionIsCanceledAsync(subscription.Id, subscription.CancelAtPeriodEnd);
        }

        /// <inheritdoc cref="IWebhookService.DeleteSubscriptionAsync(Subscription)"/>
        public async Task DeleteSubscriptionAsync(Subscription subscription)
        {
            var userId = subscription.Metadata[MetadataConstants.UserId];

            await _subscriptionRepository.UpdateSubscriptionAsync(new()
            {
                SubscriptionStripeId = subscription.Id,
                StartDate = subscription.CurrentPeriodStart,
                EndDate = subscription.CurrentPeriodEnd,
                IsActive = false,
                UserId = Guid.Parse(userId),
                IsCanceled = true
            }, null);

            await SetDefaultSubscriptionAsync(subscription.CustomerId, userId);
        }

        /// <inheritdoc cref="IWebhookService.PaymentFailedHandleAsync(Invoice)"/>
        public async Task PaymentFailedHandleAsync(Invoice invoice)
        {
            var subscription = await GetSubscriptionService().GetAsync(invoice.SubscriptionId);

            var getProductId = subscription.Items.Data.FirstOrDefault()?.Plan.ProductId
                ?? throw new Exception("Taking productId error");

            var productModel = await _productCatalogService.GetProductAsync(getProductId);
            var productName = productModel.Name.Substring(0, productModel.Name.IndexOf("/"));

            await _paymentRepository.CreatePaymentAsync(new()
            {
                Id = Guid.NewGuid(),
                InvoiceId = subscription.LatestInvoiceId,
                Mode = "subscription payment",
                ProductName = productName,
                UserId = Guid.Parse(subscription.Metadata[MetadataConstants.UserId]),
                Status = "failed",
                AmountOfPoints = 0,
                UpdatedBallance = false
            });
        }

        public async Task PaymentSucceededHandleAsync(Invoice invoice)
        {
            var subscription = await GetSubscriptionService().GetAsync(invoice.SubscriptionId);
            var userId = subscription.Metadata[MetadataConstants.UserId];

            var getProductId = subscription.Items.Data.FirstOrDefault()?.Plan.ProductId
                ?? throw new Exception("Taking productId error");

            var productModel = await _productCatalogService.GetProductAsync(getProductId);
            var productName = productModel.Name.Substring(0, productModel.Name.IndexOf("/"));

            await _subscriptionRepository.UpdateSubscriptionAsync(new()
            {
                SubscriptionStripeId = invoice.SubscriptionId,
                StartDate = subscription.CurrentPeriodStart,
                EndDate = subscription.CurrentPeriodEnd,
                IsActive = true,
                UserId = Guid.Parse(userId),
                IsCanceled = subscription.CancelAtPeriodEnd

            }, Enum.Parse<AccountPlan>(productName));

            await _paymentRepository.CreatePaymentAsync(new()
            {
                Id = Guid.NewGuid(),
                InvoiceId = subscription.LatestInvoiceId,
                Mode = "subscription update",
                ProductName = productName,
                UserId = Guid.Parse(subscription.Metadata[MetadataConstants.UserId]),
                Status = "updated",
                AmountOfPoints = 0,
                UpdatedBallance = false
            });
        }

        private async Task SetDefaultSubscriptionAsync(string customerId, string userId)
        {
            var product = (await GetProductService().SearchAsync(new()
            {
                Query = $"active:'true' AND name~'{AccountPlan.Free}'"
            })).Data.FirstOrDefault() ?? throw new Exception("Taking product for free subscription failed");

            var createdSubscription = await GetSubscriptionService().CreateAsync(new()
            {
                Customer = customerId,
                Items = new() { new() { Price = product.DefaultPriceId } },
                Metadata = new() {
                        { MetadataConstants.AccountPlan, AccountPlan.Free.ToString() },
                        { MetadataConstants.UserId, userId },
                        { MetadataConstants.ProductName,  product.Name}
                }
            });

            await _subscriptionRepository.CreateSubscriptionAsync(new DAL.Models.Subscription()
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
    }
}
