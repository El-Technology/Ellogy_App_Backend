using PaymentManager.DAL.Repositories;

namespace PaymentManager.BLL.Services;
public class WalletExternalService
{
    private readonly WalletExternalRepository _walletExternalRepository;
    public WalletExternalService(WalletExternalRepository walletExternalRepository)
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
