
namespace UserManager.DAL.Interfaces
{
    public interface IPaymentRepository
    {
        Task CreateWalletForNewUserAsync(Guid userId);
    }
}
