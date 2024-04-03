
namespace PaymentManager.DAL.Interfaces;
public interface IWalletExternalRepository
{
    Task<bool> CheckIfUserAllowedToCreateRequest(Guid userId, int userMinimum);
    Task TakeServiceFeeAsync(Guid userId, int feeAmount);
}
