using PaymentManager.BLL.Models;
using PaymentManager.DAL.Models;
using Stripe.Checkout;

namespace PaymentManager.BLL.Interfaces
{
    public interface IPaymentService
    {
        Task<SessionCreateOptions> CreatePaymentAsync(Guid userId, StreamRequest streamRequest);
        Task<List<Product>> GetAllProductsAsync();
        Task<int> GetUserBalanceAsync(Guid userId);
        Task<Wallet?> OrderConfirmationAsync(string sessionId, Guid userId);
    }
}
