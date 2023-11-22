using PaymentManager.BLL.Models;
using PaymentManager.DAL.Models;
using Stripe.Checkout;

namespace PaymentManager.BLL.Interfaces
{
    public interface IPaymentService
    {
        Task<SessionCreateOptions> CreatePaymentAsync(Guid userId, CreatePaymentRequest streamRequest);
        Task ExpireSessionAsync(string sessionId);
        Task<int> GetUserBalanceAsync(Guid userId);
        Task OrderConfirmationAsync(string sessionId);
    }
}
