using PaymentManager.BLL.Interfaces;
using PaymentManager.DAL.Interfaces;

namespace PaymentManager.BLL.Services;
public class WalletExternalService : IWalletExternalService
{
    private readonly IWalletExternalRepository _walletExternalRepository;
    public WalletExternalService(IWalletExternalRepository walletExternalRepository)
    {
        _walletExternalRepository = walletExternalRepository;
    }

    public async Task TakeServiceFeeAsync(Guid userId, int feeAmount)
    {
        await _walletExternalRepository.TakeServiceFeeAsync(userId, feeAmount);
    }

    public async Task<bool> CheckIfUserAllowedToCreateRequest(Guid userId, int userMinimum)
    {
        return await _walletExternalRepository.CheckIfUserAllowedToCreateRequest(userId, userMinimum);
    }
}
