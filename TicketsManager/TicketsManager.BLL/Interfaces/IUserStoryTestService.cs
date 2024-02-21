using TicketsManager.BLL.Dtos.UserStoryTestDtos;
using TicketsManager.BLL.Dtos.UserStoryTestDtos.GetDtos;

namespace TicketsManager.BLL.Interfaces;

public interface IUserStoryTestService
{
    /// <summary>
    ///     Add user story test
    /// </summary>
    /// <param name="userStoryTest"></param>
    /// <returns></returns>
    Task AddUserStoryTestAsync(List<CreateUserStoryTestDto> userStoryTest);

    /// <summary>
    ///     Get user story tests
    /// </summary>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    Task<List<GetUserStoryDto>> GetUserStoryTestsAsync(Guid ticketId);

    /// <summary>
    ///     Update user story test
    /// </summary>
    /// <param name="userStoryTest"></param>
    /// <returns></returns>
    Task UpdateUserStoryTestAsync(List<GetUserStoryDto> userStoryTest);

    /// <summary>
    ///     Delete user story test
    /// </summary>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    Task DeleteUserStoryTestAsync(Guid ticketId);
}