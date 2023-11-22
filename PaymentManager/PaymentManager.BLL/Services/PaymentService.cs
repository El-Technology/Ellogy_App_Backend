using Microsoft.Extensions.Logging;
using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Models;
using PaymentManager.Common.Constants;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;
using Stripe.Checkout;

namespace PaymentManager.BLL.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ILogger<PaymentService> _logger;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository _userRepository;

        private const int amountOfItems = 1;

        public PaymentService(IPaymentRepository paymentRepository, IUserRepository userRepository, ILogger<PaymentService> logger)
        {
            _logger = logger;
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<SessionCreateOptions> CreatePaymentAsync(Guid userId, CreatePaymentRequest streamRequest)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException($"User with id - {userId} was not found");

            var sessionCreateOptions = new SessionCreateOptions()
            {
                SuccessUrl = streamRequest.SuccessUrl,
                CancelUrl = streamRequest.CancelUrl,
                LineItems = new List<SessionLineItemOptions>(),
                Mode = Constants.PaymentMode,
                CustomerEmail = user.Email,
                Metadata = new Dictionary<string, string>
                {
                    { MetadataConstants.AmountOfPoint, streamRequest.AmountOfPoints.ToString() },
                    { MetadataConstants.UserId, user.Id.ToString() },
                    { MetadataConstants.ConnectionId, streamRequest.ConnectionId },
                    { MetadataConstants.SignalRMethodName, streamRequest.SignalMethodName }
                }
            };

            var sessionListItem = new SessionLineItemOptions()
            {
                PriceData = new SessionLineItemPriceDataOptions()
                {
                    Currency = Constants.ApplicationCurrency,
                    ProductData = new SessionLineItemPriceDataProductDataOptions()
                    {
                        Name = $"{streamRequest.AmountOfPoints} - points"
                    },
                    UnitAmountDecimal = (decimal)(streamRequest.AmountOfPoints * PointPriceConstant.OneTokenPrice * PointPriceConstant.PriceInCents),
                },
                Quantity = amountOfItems
            };
            sessionCreateOptions.LineItems.Add(sessionListItem);

            return sessionCreateOptions;
        }

        public async Task OrderConfirmationAsync(string sessionId)
        {
            var service = new SessionService();
            Session session = service.Get(sessionId);

            var payment = await _paymentRepository.GetPaymentAsync(session.Id)
                ?? throw new Exception($"We can`t find payment with this session id => {session.Id}");

            if (payment.Status == "created" && !payment.UpdatedBallance)
            {
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
            }
        }

        public async Task ExpireSessionAsync(string sessionId)
        {
            var payment = await _paymentRepository.GetPaymentAsync(sessionId)
                ?? throw new Exception($"We can`t find payment with this session id => {sessionId}");

            if (payment.Status == "expired")
                return;

            var service = new SessionService();
            Session session = await service.GetAsync(sessionId);

            if (session.Status != "expired")
                await service.ExpireAsync(sessionId);

            await _paymentRepository.UpdatePaymentAsync(new Payment
            {
                PaymentId = session.PaymentIntentId,
                AmountOfPoints = int.Parse(session.Metadata[MetadataConstants.AmountOfPoint]),
                Status = session.Status,
                SessionId = session.Id,
                UpdatedBallance = false,
            });

            _logger.LogInformation($"{sessionId} - was expired");
        }

        public async Task<int> GetUserBalanceAsync(Guid userId)
        {
            var userWallet = await _paymentRepository.GetUserWalletAsync(userId)
                ?? throw new Exception($"We can`t find wallet by userId => {userId}");

            return userWallet.Balance;
        }
    }
}
