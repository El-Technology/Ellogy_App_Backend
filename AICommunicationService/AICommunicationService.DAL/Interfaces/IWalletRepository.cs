namespace AICommunicationService.DAL.Interfaces
{
    public interface IWalletRepository
    {
        Task<bool> CheckIfUserAllowedToCreateRequest(Guid userId, int userMinimum);
        Task TakeServiceFeeAsync(Guid userId, int feeAmount);
    }
}
