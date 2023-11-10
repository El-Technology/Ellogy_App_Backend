using PaymentManager.Common.Constants;
using PaymentManager.DAL.Models;
using PaymentManager.DAL.Repositories;
using Stripe.Checkout;

namespace PaymentManager.BLL.Services
{
    public class PaymentService
    {
        private readonly TestRepo _testRepo;
        public PaymentService(TestRepo testRepo)
        {
            _testRepo = testRepo;
        }

        public async Task<SessionCreateOptions> CreatePaymentAsync(Guid productId, Guid userId)
        {
            var user = await _testRepo.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException($"User with id - {userId} was not found");

            if ((await _testRepo.GetUserWalletAsync(user.Id)) is null)
                await _testRepo.CreateUserWalletAsync(user.Id);

            var product = await _testRepo.GetProductByIdAsync(productId);

            var options = new SessionCreateOptions()
            {
                SuccessUrl = $"https://localhost:7267/api/CheckOut/OrderConfirmation",
                CancelUrl = $"https://localhost:7267/api/CheckOut/OrderConfirmation",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = Constants.PaymentMode,
                CustomerEmail = user.Email,
                Metadata = new Dictionary<string, string>
                {
                    { "productId", productId.ToString() },
                    { "userId", user.Id.ToString() }
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
                Quantity = 1
            };
            options.LineItems.Add(sessionListItem);

            return options;
        }

        public async Task<Wallet?> OrderConfirmationAsync(string sessionId, Guid userId)
        {
            var user = await _testRepo.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException($"User with id - {userId} was not found");

            var service = new SessionService();
            Session session = service.Get(sessionId);

            var updatedBalance = await _testRepo.GetPaymentAsync(session.Id);

            if (session.Status == "complete" && !updatedBalance.UpdatedBallance)
            {
                await _testRepo.UpdateBalance(user.Id, Guid.Parse(session.Metadata["productId"]));
                await _testRepo.UpdatePaymentAsync(new Payment
                {
                    PaymentId = session.PaymentIntentId,
                    ProductId = Guid.Parse(session.Metadata["productId"]),
                    Status = session.Status,
                    UserEmail = session.CustomerEmail,
                    UserId = user.Id,
                    SessionId = session.Id,
                    UpdatedBallance = true
                });
            }

            return await _testRepo.GetUserWalletAsync(user.Id);
        }
    }
}
