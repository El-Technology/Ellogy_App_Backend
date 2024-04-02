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
    Task<List<GetUserStoryDto>> AddUserStoryTestAsync(List<CreateUserStoryTestDto> userStoryTest);

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
    Task UpdateUserStoryTestAsync(List<UpdateUserStoryTestDto> userStoryTest);

    /// <summary>
    ///     Delete user story test
    /// </summary>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    Task DeleteUserStoryTestAsync(Guid ticketId);

    /// <summary>
    ///    Delete test cases by ids
    /// </summary>
    /// <param name="listOfTestCaseIds"></param>
    /// <returns></returns>
    Task DeleteTestCasesByIdsAsync(List<Guid> listOfTestCaseIds);
}