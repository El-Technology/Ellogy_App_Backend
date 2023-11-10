using PaymentManager.BLL.Models;
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

        public async Task<SessionCreateOptions> CreatePaymentAsync(CreatePaymentDto paymentDto)
        {
            if ((await _testRepo.GetUserWalletAsync(paymentDto.UserEmail)) is null)
                await _testRepo.CreateUserWalletAsync(paymentDto.UserEmail);

            var product = await _testRepo.GetProductByIdAsync(paymentDto.ProductId);

            var options = new SessionCreateOptions()
            {
                SuccessUrl = $"https://localhost:7267/api/CheckOut/OrderConfirmation",
                CancelUrl = $"https://localhost:7267/api/CheckOut/OrderConfirmation",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = paymentDto.UserEmail,
                Metadata = new Dictionary<string, string>
                {
                    { "productId", paymentDto.ProductId.ToString() }
                }
            };

            var sessionListItem = new SessionLineItemOptions()
            {
                PriceData = new SessionLineItemPriceDataOptions()
                {
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions()
                    {
                        Name = product.Title
                    },
                    UnitAmount = product.Price * 100,
                },
                Quantity = 1
            };
            options.LineItems.Add(sessionListItem);

            return options;
        }

        public async Task<Wallet?> OrderConfirmationAsync(string sessionId)
        {
            var service = new SessionService();
            Session session = service.Get(sessionId);

            var updatedBalance = await _testRepo.GetPaymentAsync(session.Id);

            if (session.Status == "complete" && !updatedBalance.UpdatedBallance)
            {
                await _testRepo.UpdateBalance(session.CustomerEmail, Guid.Parse(session.Metadata["productId"]));
                await _testRepo.UpdatePaymentAsync(new DAL.Models.Payment
                {
                    PaymentId = session.PaymentIntentId,
                    ProductId = Guid.Parse(session.Metadata["productId"]),
                    Status = session.Status,
                    UserEmail = session.CustomerEmail,
                    SessionId = session.Id,
                    UpdatedBallance = true
                });
            }

            return await _testRepo.GetUserWalletAsync(session.CustomerEmail);
        }
    }
}
