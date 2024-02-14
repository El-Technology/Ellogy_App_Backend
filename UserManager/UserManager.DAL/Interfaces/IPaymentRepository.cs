namespace UserManager.DAL.Interfaces;

public interface IPaymentRepository
{
    /// <summary>
    ///     Create wallet for new user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task CreateWalletForNewUserAsync(Guid userId);

    /// <summary>
    ///     Check if user have wallet
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<bool> CheckIfUserHaveWalletAsync(Guid userId);
}