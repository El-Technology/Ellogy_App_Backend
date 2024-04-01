using AICommunicationService.BLL.Dtos;

namespace AICommunicationService.BLL.Interfaces.HttpInterfaces;
public interface IPaymentExternalHttpService
{
    Task<List<UserDto>> GetUsersByIdsAsync(List<Guid> userIds);
}
