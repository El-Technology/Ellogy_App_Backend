namespace AICommunicationService.BLL.Interfaces.HttpInterfaces;
public interface IPaymentExternalHttpService
{
    Task<bool> CheckIfUserAllowedToCreateRequestAsync(Guid userId, int userMinimum);
    Task TakeServiceFeeAsync(Guid userId, int feeAmount);
}
