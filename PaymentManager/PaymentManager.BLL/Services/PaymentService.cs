using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Models;
using PaymentManager.Common.Constants;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;
using PaymentManager.DAL.Repositories;
using Stripe.Checkout;

namespace PaymentManager.BLL.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository _userRepository;

        private const int amountOfItems = 1;

        public PaymentService(IPaymentRepository paymentRepository, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<SessionCreateOptions> CreatePaymentAsync(Guid userId, StreamRequest streamRequest)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException($"User with id - {userId} was not found");

            var product = await _paymentRepository.GetProductByIdAsync(streamRequest.ProductId)
                ?? throw new Exception("Wrong product id");

            var sessionCreateOptions = new SessionCreateOptions()
            {
                SuccessUrl = $"https://localhost:7267/api/CheckOut/OrderConfirmation", //will be changed
                CancelUrl = $"https://localhost:7267/api/CheckOut/OrderConfirmation", //will be changed
                LineItems = new List<SessionLineItemOptions>(),
                Mode = Constants.PaymentMode,
                CustomerEmail = user.Email,
                Metadata = new Dictionary<string, string>
                {
                    { MetadataConstants.ProductId, streamRequest.ProductId.ToString() },
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
                        Name = product.Title
                    },
                    UnitAmount = product.Price * Constants.PriceInCents,
                },
                Quantity = amountOfItems
            };
            sessionCreateOptions.LineItems.Add(sessionListItem);

            return sessionCreateOptions;
        }

        public async Task<Wallet?> OrderConfirmationAsync(string sessionId, Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException($"User with id - {userId} was not found");

            var service = new SessionService();
            Session session = service.Get(sessionId);

            var payment = await _paymentRepository.GetPaymentAsync(session.Id)
                ?? throw new Exception($"We can`t find payment with this session id => {session.Id}");

            if (session.Status == "complete" && !payment.UpdatedBallance)
            {
                await _paymentRepository.UpdateBalance(user.Id, Guid.Parse(session.Metadata[MetadataConstants.ProductId]));
                await _paymentRepository.UpdatePaymentAsync(new Payment
                {
                    PaymentId = session.PaymentIntentId,
                    ProductId = Guid.Parse(session.Metadata[MetadataConstants.ProductId]),
                    Status = session.Status,
                    UserEmail = session.CustomerEmail,
                    UserId = user.Id,
                    SessionId = session.Id,
                    UpdatedBallance = true
                });
            }

            return await _paymentRepository.GetUserWalletAsync(user.Id);
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _paymentRepository.GetAllProductsAsync();
        }

        public async Task<int> GetUserBalanceAsync(Guid userId)
        {
            var userWallet = await _paymentRepository.GetUserWalletAsync(userId)
                ?? throw new Exception($"We can`t find wallet by userId => {userId}");

            return userWallet.Balance;
        }
    }
}
