using TicketsManager.DAL.Dtos;
using TicketsManager.DAL.Models.UserStoryTests;

namespace TicketsManager.DAL.Interfaces;

public interface IUserStoryTestRepository
{
    /// <summary>
    ///     Method adds array of UserStoryTest to the database
    /// </summary>
    /// <param name="userStoryTests"></param>
    /// <returns></returns>
    Task AddUserStoryTestAsync(List<UserStoryTest> userStoryTests);

    /// <summary>
    ///     Method returns UserStoryTest by ticketId
    /// </summary>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    IQueryable<ReturnUserStoryTestModel> GetUserStoryTests(Guid ticketId);

    /// <summary>
    ///     Method updates array of UserStoryTest in the database
    /// </summary>
    /// <param name="userStoryTests"></param>
    /// <returns></returns>
    Task UpdateUserStoryTestAsync(List<UserStoryTest> userStoryTests);

    /// <summary>
    ///     Method deletes UserStoryTest by ticketId
    /// </summary>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    Task DeleteUserStoryTestByTicketIdAsync(Guid ticketId);

    /// <summary>
    ///    Method deletes TestCases by list of TestCaseIds
    /// </summary>
    /// <param name="listOfTestCaseIds"></param>
    /// <returns></returns>
    Task DeleteTestCasesByIds(List<Guid> listOfTestCaseIds);

    /// <summary>
    ///    Method returns UserStoryTest by list of UserStoryTest
    /// </summary>
    /// <param name="userStoryTests"></param>
    /// <returns></returns>
    IQueryable<ReturnUserStoryTestModel> GetUserStoryTests(List<UserStoryTest> userStoryTests);
}