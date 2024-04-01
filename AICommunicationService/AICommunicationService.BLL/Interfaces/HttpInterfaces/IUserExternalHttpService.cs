using AICommunicationService.BLL.Dtos;
using AICommunicationService.Common.Dtos;

namespace AICommunicationService.BLL.Interfaces.HttpInterfaces;
public interface IUserExternalHttpService
{
    Task<List<UserDto>> FindUserByEmailAsync(string email);
    Task<int> GetTotalPointsUsageAsync(Guid userId);
    Task<UserDto> GetUserByIdAsync(Guid userId);
    Task<List<UserDto>> GetUsersByIdsAsync(List<Guid> userIds);
    Task<PaginationResponseDto<UserDto>> GetUsersByIdsWithPaginationAsync(List<Guid> userIds, PaginationRequestDto paginationRequest);
    Task UpdateUserTotalPointsUsageAsync(Guid userId, int usedTokens);
}
