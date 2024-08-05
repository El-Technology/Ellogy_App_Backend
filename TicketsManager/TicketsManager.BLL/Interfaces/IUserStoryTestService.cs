using TicketsManager.BLL.Dtos.UserStoryTestDtos;
using TicketsManager.BLL.Dtos.UserStoryTestDtos.GetDtos;
using TicketsManager.Common.Dtos;

namespace TicketsManager.BLL.Interfaces;

public interface IUserStoryTestService
{
    /// <summary>
    ///     Add user story test
    /// </summary>
    /// <param name="userStoryTest"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<GetUserStoryDto>> AddUserStoryTestAsync(Guid userId, List<CreateUserStoryTestDto> userStoryTest);

    /// <summary>
    ///     Get user story tests
    /// </summary>
    /// <param name="ticketId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<GetUserStoryDto>> GetUserStoryTestsAsync(Guid userId, Guid ticketId);

    /// <summary>
    ///     Update user story test
    /// </summary>
    /// <param name="userStoryTest"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task UpdateUserStoryTestAsync(Guid userId, List<UpdateUserStoryTestDto> userStoryTest);

    /// <summary>
    ///     Delete user story test
    /// </summary>
    /// <param name="ticketId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task DeleteUserStoryTestAsync(Guid userId, Guid ticketId);

    /// <summary>
    ///    Delete test cases by ids
    /// </summary>
    /// <param name="listOfTestCaseIds"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task DeleteTestCasesByIdsAsync(Guid userId, List<Guid> listOfTestCaseIds);
    Task<PaginationResponseDto<GetUserStoryDto>> GetUserStoryTestsAsync(Guid userId, Guid ticketId, PaginationRequestDto paginationRequest);
}