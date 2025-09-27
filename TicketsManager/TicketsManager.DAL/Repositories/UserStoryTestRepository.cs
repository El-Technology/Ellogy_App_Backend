using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Context;
using TicketsManager.DAL.Dtos;
using TicketsManager.DAL.Extensions;
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
        .Include(a => a.RelatedSummaries)
        .Where(filterExpression)
        .OrderBy(a => a.Order)
        .Select(a => new ReturnUserStoryTestModel
        {
            Id = a.Id,
            TestScenarios = a.TestScenarios,
            TestCases = a.TestCases,
            TestPlan = a.TestPlan,
            UsecaseId = a.UsecaseId,
            UsecaseTitle = a.Usecase!.Title,
            RelatedSummaries = a.RelatedSummaries
                .Select(rs => new UserStoryTestTicketSummary
                {
                    Id = rs.Id,
                    TicketSummaryId = rs.TicketSummaryId,
                    UserStoryTestId = rs.UserStoryTestId,
                    CreatedAt = rs.CreatedAt
                })
                .ToList()
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

    /// <inheritdoc cref="IUserStoryTestRepository.GetUserStoryTestsAsync" />
    public async Task<PaginationResponseDto<ReturnUserStoryTestModel>> GetUserStoryTestsAsync(
        Guid ticketId, PaginationRequestDto paginationRequest)
    {
        return await GetUserStoryTestQueryWithFilter(a => a.Usecase!.TicketId == ticketId)
            .GetFinalResultAsync(paginationRequest);
    }

    /// <inheritdoc cref="IUserStoryTestRepository.UpdateUserStoryTestAsync" />
    public async Task UpdateUserStoryTestAsync(List<UserStoryTest> userStoryTests)
    {
        _context.UserStoryTests.UpdateRange(userStoryTests);
        await _context.SaveChangesAsync();
    }

    public async Task ReplaceRelatedSummariesAsync(Guid userStoryTestId, IEnumerable<Guid> ticketSummaryIds)
    {
        var existing = await _context.UserStoryTestTicketSummaries
            .Where(rs => rs.UserStoryTestId == userStoryTestId)
            .ToListAsync();

        var desired = ticketSummaryIds.Distinct().ToHashSet();

        var toRemove = existing.Where(rs => !desired.Contains(rs.TicketSummaryId)).ToList();
        if (toRemove.Count > 0)
        {
            _context.UserStoryTestTicketSummaries.RemoveRange(toRemove);
        }

        var existingIds = existing.Select(rs => rs.TicketSummaryId).ToHashSet();
        var toAdd = desired
            .Where(id => !existingIds.Contains(id))
            .Select(id => new UserStoryTestTicketSummary
            {
                Id = Guid.NewGuid(),
                UserStoryTestId = userStoryTestId,
                TicketSummaryId = id
            })
            .ToList();

        if (toAdd.Count > 0)
        {
            await _context.UserStoryTestTicketSummaries.AddRangeAsync(toAdd);
        }

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
