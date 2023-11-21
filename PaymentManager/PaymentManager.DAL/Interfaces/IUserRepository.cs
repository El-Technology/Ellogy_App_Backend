using PaymentManager.DAL.Models;

namespace PaymentManager.DAL.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(Guid userId);
        Task UpdateTotalPurchasedTokensAsync(Guid userId, int purchasedTokens);
    }
}
