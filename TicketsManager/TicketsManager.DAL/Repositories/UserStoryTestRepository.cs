using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TicketsManager.DAL.Context;
using TicketsManager.DAL.Dtos;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.UserStoryTestsModels;

namespace TicketsManager.DAL.Repositories;

public class UserStoryTestRepository : IUserStoryTestRepository
{
    private readonly TicketsManagerDbContext _context;

    public UserStoryTestRepository(TicketsManagerDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> GetTicketIdByTestCaseIdAsync(Guid testCaseId)
    {
        return await _context.TestCases
            .Where(tc => tc.Id == testCaseId)
            .Select(tc => tc.UserStoryTest!.Usecase!.TicketId)
            .FirstOrDefaultAsync();
    }

    public async Task<Guid> GetTicketIdByUsecaseIdAsync(Guid usecaseId)
    {
        return await _context.Usecases
            .Where(u => u.Id == usecaseId)
            .Select(u => u.TicketId)
            .FirstOrDefaultAsync();
    }

    public async Task<Dictionary<Guid, Guid>> GetUsecaseTicketIdRelationAsync(List<Guid> usecaseIds)
    {
        return await _context.Usecases
            .Where(u => usecaseIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.TicketId);
    }

    public async Task<int> GetLastOrderForStoryTestByTicketIdAsync(Guid ticketId)
    {
        return await _context.UserStoryTests
            .Where(a => a.Usecase!.TicketId == ticketId)
            .Select(a => a.Order)
            .DefaultIfEmpty()
            .MaxAsync();
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
        .OrderBy(a => a.Order)
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