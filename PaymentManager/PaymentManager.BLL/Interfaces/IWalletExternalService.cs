
namespace PaymentManager.BLL.Interfaces;
public interface IWalletExternalService
{
    Task<bool> CheckIfUserAllowedToCreateRequest(Guid userId, int userMinimum);
    Task TakeServiceFeeAsync(Guid userId, int feeAmount);
}
