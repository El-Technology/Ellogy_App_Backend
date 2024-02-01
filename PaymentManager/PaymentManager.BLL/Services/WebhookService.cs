using Microsoft.Extensions.Logging;
using PaymentManager.BLL.Interfaces;
using PaymentManager.Common.Constants;
using PaymentManager.DAL.Enums;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;
using Stripe;
using Stripe.Checkout;


namespace PaymentManager.BLL.Services
{
    public class WebhookService : IWebhookService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ILogger<WebhookService> _logger;

        public WebhookService(IPaymentRepository paymentRepository,
            IUserRepository userRepository,
            ISubscriptionRepository subscriptionRepository,
            ILogger<WebhookService> logger)
        {
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

            if (payment.UpdatedBallance && payment.Mode.Equals(Constants.PAYMENT_MODE))
                return;

            switch (session.Mode)
            {
                case Constants.PAYMENT_MODE:
                    await _paymentRepository.UpdatePaymentAsync(new Payment
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
                    await _paymentRepository.UpdatePaymentAsync(new Payment
                    {
                        InvoiceId = session.InvoiceId,
                        AmountOfPoints = int.Parse(session.Metadata[MetadataConstants.AmountOfPoint]),
                        Status = session.Status,
                        UserEmail = session.CustomerEmail,
                        UserId = payment.UserId,
                        SessionId = session.Id,
                        UpdatedBallance = true,
                    });

                    var expandedSession = new SessionService();

                    var result = await expandedSession.GetAsync(session.Id, new SessionGetOptions
                    {
                        Expand = new() { "subscription" }
                    });

                    await _subscriptionRepository.CreateSubscriptionAsync(new()
                    {
                        Id = Guid.NewGuid(),
                        StartDate = result.Subscription.CurrentPeriodStart,
                        EndDate = result.Subscription.CurrentPeriodEnd,
                        IsActive = true,
                        UserId = payment.UserId,
                        SubscriptionStripeId = result.SubscriptionId,
                        IsCanceled = false
                    },
                    Enum.Parse<AccountPlan>(result.Subscription.Metadata[MetadataConstants.AccountPlan]));
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

            var service = new SessionService();

            if (session.Status != "expired")
                await service.ExpireAsync(session.Id);

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

        /// <inheritdoc cref="IWebhookService.UpdateSubscriptionAsync(Stripe.Subscription)"/>
        public async Task UpdateSubscriptionAsync(Stripe.Subscription subscription)
        {
            await _paymentRepository.CreatePaymentAsync(new()
            {
                Id = Guid.NewGuid(),
                InvoiceId = subscription.LatestInvoiceId,
                Mode = "subscription payment",
                ProductName = subscription.Metadata[MetadataConstants.ProductName],
                UserId = Guid.Parse(subscription.Metadata[MetadataConstants.UserId]),
                Status = "updated",
                AmountOfPoints = 0,
                UpdatedBallance = true
            });

            await _subscriptionRepository.UpdateSubscriptionAsync(new()
            {
                SubscriptionStripeId = subscription.Id,
                StartDate = subscription.CurrentPeriodStart,
                EndDate = subscription.CurrentPeriodEnd,
                IsActive = true,
                UserId = Guid.Parse(subscription.Metadata[MetadataConstants.UserId]),
                IsCanceled = subscription.CancelAtPeriodEnd

            }, Enum.Parse<AccountPlan>(subscription.Metadata[MetadataConstants.AccountPlan]));
        }

        /// <inheritdoc cref="IWebhookService.DeleteSubscriptionAsync(Stripe.Subscription)"/>
        public async Task DeleteSubscriptionAsync(Stripe.Subscription subscription)
        {
            await _subscriptionRepository.UpdateSubscriptionAsync(new()
            {
                SubscriptionStripeId = subscription.Id,
                StartDate = subscription.CurrentPeriodStart,
                EndDate = subscription.CurrentPeriodEnd,
                IsActive = false,
                UserId = Guid.Parse(subscription.Metadata[MetadataConstants.UserId]),
                IsCanceled = true

            }, AccountPlan.Free);
        }

        /// <inheritdoc cref="IWebhookService.PaymentFailedHandleAsync(Invoice)"/>
        public async Task PaymentFailedHandleAsync(Invoice invoice)
        {
            var subscriptionService = new SubscriptionService();
            var subscription = await subscriptionService.GetAsync(invoice.SubscriptionId);

            await _subscriptionRepository.UpdateSubscriptionAsync(new()
            {
                SubscriptionStripeId = invoice.SubscriptionId,
                StartDate = subscription.CurrentPeriodStart,
                EndDate = subscription.CurrentPeriodEnd,
                IsActive = false,
                UserId = Guid.Parse(subscription.Metadata[MetadataConstants.UserId]),
                IsCanceled = true

            }, AccountPlan.Free);
        }
    }
}
