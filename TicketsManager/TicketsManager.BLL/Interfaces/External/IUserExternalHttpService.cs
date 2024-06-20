using TicketsManager.BLL.Dtos;

namespace TicketsManager.BLL.Interfaces.External;
public interface IUserExternalHttpService
{
    Task<List<UserDto>> FindUserByEmailAsync(string email);
    Task<UserDto> GetUserByIdAsync(Guid userId);
    Task<List<UserDto>> GetUsersByIdsAsync(List<Guid> userIds);
}