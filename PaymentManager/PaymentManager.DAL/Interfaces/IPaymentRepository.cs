using PaymentManager.DAL.Models;

namespace PaymentManager.DAL.Interfaces
{
    public interface IPaymentRepository
    {
        Task CreatePaymentAsync(Payment payment);
        Task<Wallet> CreateUserWalletAsync(Guid userId);
        Task<Payment?> GetPaymentAsync(string sessionId);
        Task<Payment?> GetPaymentByIdAsync(string paymentId);
        Task<Wallet?> GetUserWalletAsync(Guid userId);
        Task UpdateBalanceAsync(Guid userId, int amountOfPoints);
        Task UpdatePaymentAsync(Payment payment);
    }
}
