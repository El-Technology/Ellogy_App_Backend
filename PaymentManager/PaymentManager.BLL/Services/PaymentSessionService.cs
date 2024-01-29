using Microsoft.Extensions.Logging;
using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Models;
using PaymentManager.Common.Constants;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;
using Stripe;
using Stripe.Checkout;

namespace PaymentManager.BLL.Services
{
    public class PaymentSessionService : IPaymentSessionService
    {
        private readonly ILogger<PaymentSessionService> _logger;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository _userRepository;

        private const int amountOfItems = 1;

        public PaymentSessionService(IPaymentRepository paymentRepository, IUserRepository userRepository, ILogger<PaymentSessionService> logger)
        {
            _logger = logger;
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
        }

        /// <inheritdoc cref="IPaymentSessionService.CreateOneTimePaymentAsync(Guid, CreatePaymentRequest)"/>
        public async Task<SessionCreateOptions> CreateOneTimePaymentAsync(Guid userId, CreatePaymentRequest streamRequest)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException($"User with id - {userId} was not found");

            var sessionCreateOptions = new SessionCreateOptions()
            {
                SuccessUrl = streamRequest.SuccessUrl,
                CancelUrl = streamRequest.CancelUrl,
                LineItems = new List<SessionLineItemOptions>()
                {
                    new()
                    {
                        PriceData = new()
                        {
                            Currency = Constants.ApplicationCurrency,
                            ProductData = new()
                            {
                                Name = $"{streamRequest.AmountOfPoints} - points"
                            },
                            UnitAmountDecimal = (decimal)(streamRequest.AmountOfPoints * PointPriceConstant.OneTokenPrice * PointPriceConstant.PriceInCents),
                        },
                        Quantity = amountOfItems
                    }
                },
                Mode = Constants.PAYMENT_MODE,
                CustomerEmail = user.Email,
                Metadata = new Dictionary<string, string>
                {
                    { MetadataConstants.AmountOfPoint, streamRequest.AmountOfPoints.ToString() },
                    { MetadataConstants.UserId, user.Id.ToString() },
                    { MetadataConstants.ConnectionId, streamRequest.ConnectionId },
                    { MetadataConstants.SignalRMethodName, streamRequest.SignalMethodName }
                }
            };

            return sessionCreateOptions;
        }

        /// <inheritdoc cref="IPaymentSessionService.OrderConfirmationAsync(string)"/>
        public async Task OrderConfirmationAsync(Session session)
        {
            var payment = await _paymentRepository.GetPaymentAsync(session.Id);

            if (payment is null)
            {
                Console.WriteLine("setup");
                return;
            }

            if (payment.UpdatedBallance)
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
                    Console.WriteLine("Need confirm the subscription");
                    break;

                case Constants.SETUP_MODE:
                    Console.WriteLine("Need confirm the setup");
                    break;

                default:
                    throw new Exception("Session confirmation error");
            }
        }

        /// <inheritdoc cref="IPaymentSessionService.ExpireSessionAsync(Session)"/>
        public async Task ExpireSessionAsync(Session session)
        {
            var payment = await _paymentRepository.GetPaymentAsync(session.Id);

            if (payment is null)
            {
                Console.WriteLine("setup");
                return;
            }

            if (payment.Status == "expired")
                return;

            var service = new SessionService();

            if (session.Status != "expired")
                await service.ExpireAsync(session.Id);

            await _paymentRepository.UpdatePaymentAsync(new Payment
            {
                PaymentId = session.PaymentIntentId,
                AmountOfPoints = int.Parse(session.Metadata[MetadataConstants.AmountOfPoint]),
                Status = session.Status,
                SessionId = session.Id,
                UpdatedBallance = false,
            });

            _logger.LogInformation($"{session.Id} - was expired");
        }

        /// <inheritdoc cref="IPaymentSessionService.GetUserBalanceAsync(Guid)"/>
        public async Task<int> GetUserBalanceAsync(Guid userId)
        {
            var userWallet = await _paymentRepository.GetUserWalletAsync(userId)
                ?? throw new Exception($"We can`t find wallet by userId => {userId}");

            return userWallet.Balance;
        }

        // --------------------------------------------- subscriptions

        public async Task<SessionCreateOptions> CreateSubscriptionAsync(CreateSubscriptionRequest createSubscriptionRequest, Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException(nameof(userId));

            var customerService = new CustomerService();
            var customerData = await customerService.GetAsync(user.StripeCustomerId,
                                new CustomerGetOptions
                                {
                                    Expand = new() { "subscriptions" }
                                })
                ?? throw new Exception("We can`t find you payment profile");

            if (customerData.Subscriptions is not null && customerData.Subscriptions.Any())
                throw new Exception("You have to cancel your existing subscription before having a new one");

            var sessionCreateOptions = new SessionCreateOptions()
            {
                SuccessUrl = createSubscriptionRequest.SuccessUrl,
                CancelUrl = createSubscriptionRequest.CancelUrl,
                Customer = user.StripeCustomerId,
                Mode = Constants.SUBSCRIPTION_MODE,
                LineItems = new List<SessionLineItemOptions>
                {
                    new ()
                    {
                        Price = createSubscriptionRequest.PriceId,
                        Quantity = 1,
                    },
                },
                Metadata = new Dictionary<string, string>
                {
                    { MetadataConstants.AmountOfPoint, "0" },
                    { MetadataConstants.UserId, user.Id.ToString() },
                    { MetadataConstants.ConnectionId, createSubscriptionRequest.ConnectionId },
                    { MetadataConstants.SignalRMethodName, createSubscriptionRequest.SignalMethodName }
                }
            };

            return sessionCreateOptions;
        }

        public async Task CancelSubscriptionAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException(nameof(userId));

            var customerService = new CustomerService();
            var customerData = await customerService.GetAsync(user.StripeCustomerId,
                                new CustomerGetOptions
                                {
                                    Expand = new() { "subscriptions" }
                                })
                ?? throw new Exception("We can`t find you payment profile");

            if (customerData.Subscriptions is null && !customerData.Subscriptions!.Any())
                throw new Exception("You don`t have any subscription");

            var subscriptionService = new SubscriptionService();

            var subscriptionUpdateOptions = new SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = true
            };

            await subscriptionService.UpdateAsync(customerData.Subscriptions!.FirstOrDefault()?.Id, subscriptionUpdateOptions);
        }
    }
}
