using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TicketsManager.DAL.Context;
using TicketsManager.DAL.Dtos;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.UserStoryTests;

namespace TicketsManager.DAL.Repositories;

public class UserStoryTestRepository : IUserStoryTestRepository
{
    private readonly TicketsManagerDbContext _context;

    public UserStoryTestRepository(TicketsManagerDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc cref="IUserStoryTestRepository.AddUserStoryTestAsync" />
    public async Task AddUserStoryTestAsync(List<UserStoryTest> userStoryTests)
    {
        await _context.UserStoryTests.AddRangeAsync(userStoryTests);
        await _context.SaveChangesAsync();
    }

    private IQueryable<ReturnUserStoryTestModel> GetUserStoryTestQueryWithFilter(
        Expression<Func<UserStoryTest, bool>> filterExpression)
    {
        return _context.UserStoryTests
        .Include(a => a.TestCases)
        .Include(a => a.TestPlan)
        .Where(filterExpression)
        .Select(a => new ReturnUserStoryTestModel
        {
            Id = a.Id,
            TestScenarios = a.TestScenarios,
            TestCases = a.TestCases,
            TestPlan = a.TestPlan,
            UsecaseId = a.UsecaseId,
            UsecaseTitle = a.Usecase!.Title
        });
    }

    /// <inheritdoc cref="IUserStoryTestRepository.GetUserStoryTests" />
    public IQueryable<ReturnUserStoryTestModel> GetUserStoryTests(List<UserStoryTest> userStoryTests)
    {
        return GetUserStoryTestQueryWithFilter(a => userStoryTests
            .Select(ust => ust.UsecaseId)
            .Contains(a.UsecaseId));
    }

    /// <inheritdoc cref="IUserStoryTestRepository.GetUserStoryTests" />
    public IQueryable<ReturnUserStoryTestModel> GetUserStoryTests(Guid ticketId)
    {
        return GetUserStoryTestQueryWithFilter(a => a.Usecase!.TicketId == ticketId);
    }

    /// <inheritdoc cref="IUserStoryTestRepository.UpdateUserStoryTestAsync" />
    public async Task UpdateUserStoryTestAsync(List<UserStoryTest> userStoryTests)
    {
        _context.UserStoryTests.UpdateRange(userStoryTests);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="IUserStoryTestRepository.DeleteUserStoryTestByTicketIdAsync" />
    public async Task DeleteUserStoryTestByTicketIdAsync(Guid ticketId)
    {
        await _context.UserStoryTests
            .Where(a => a.Usecase!.TicketId == ticketId)
            .ExecuteDeleteAsync();
    }

    /// <inheritdoc cref="IUserStoryTestRepository.DeleteTestCasesByIds" />
    public async Task DeleteTestCasesByIds(List<Guid> listOfTestCaseIds)
    {
        await _context.TestCases
            .Where(tc => listOfTestCaseIds.Contains(tc.Id))
            .ExecuteDeleteAsync();
    }
}