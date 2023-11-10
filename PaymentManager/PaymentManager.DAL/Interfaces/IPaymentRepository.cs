using PaymentManager.DAL.Models;

namespace PaymentManager.DAL.Interfaces
{
    public interface IPaymentRepository
    {
        Task CreatePaymentAsync(Payment payment);
        Task<Wallet> CreateUserWalletAsync(Guid userId);
        Task<List<Product>> GetAllProductsAsync();
        Task<Payment?> GetPaymentAsync(string sessionId);
        Task<Product?> GetProductByIdAsync(Guid productId);
        Task<Wallet?> GetUserWalletAsync(Guid userId);
        Task UpdateBalance(Guid userId, Guid productId);
        Task UpdatePaymentAsync(Payment payment);
    }
}
