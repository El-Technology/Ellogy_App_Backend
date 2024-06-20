using Microsoft.EntityFrameworkCore;
using TicketsManager.DAL.Context;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.TicketSummaryModels;

namespace TicketsManager.DAL.Repositories;

public class TicketSummaryRepository : ITicketSummaryRepository
{
    private readonly TicketsManagerDbContext _context;

    public TicketSummaryRepository(TicketsManagerDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc cref="ITicketSummaryRepository.CreateTicketSummariesAsync" />
    public async Task CreateTicketSummariesAsync(List<TicketSummary> ticketSummaries)
    {
        await _context.AddRangeAsync(ticketSummaries);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="ITicketSummaryRepository.GetTicketSummariesByTicketIdAsync" />
    public IQueryable<TicketSummary> GetTicketSummariesByTicketIdAsync(Guid ticketId)
    {
        return _context.TicketSummaries
            .Include(a => a.SummaryAcceptanceCriteria)
            .Include(a => a.SummaryScenarios)
            .Where(a => a.TicketId == ticketId);
    }

    /// <inheritdoc cref="ITicketSummaryRepository.UpdateTicketSummariesAsync" />
    public async Task UpdateTicketSummariesAsync(List<TicketSummary> ticketSummaries)
    {
        _context.UpdateRange(ticketSummaries);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="ITicketSummaryRepository.DeleteTicketSummariesAsync" />
    public async Task DeleteTicketSummariesAsync(Guid ticketId)
    {
        await _context.TicketSummaries.Where(a => a.TicketId == ticketId).ExecuteDeleteAsync();
    }

    /// <inheritdoc cref="ITicketSummaryRepository.GetTicketSummariesByIdsAsync" />
    public async Task DeleteTicketSummaryScenariosAsync(List<Guid> summaryScenarioIds)
    {
        await _context.SummaryScenarios
            .Where(a => summaryScenarioIds.Contains(a.Id))
            .ExecuteDeleteAsync();
    }

    /// <inheritdoc cref="ITicketSummaryRepository.DeleteTicketSummariesByIdAsync" />
    public async Task DeleteTicketSummaryAcceptanceCriteriaAsync(List<Guid> summaryAcceptanceCriteriaIds)
    {
        await _context.SummaryAcceptanceCriteria
            .Where(a => summaryAcceptanceCriteriaIds.Contains(a.Id))
            .ExecuteDeleteAsync();
    }

    /// <inheritdoc cref="ITicketSummaryRepository.DeleteTicketSummariesByIdAsync" />
    public async Task DeleteTicketSummariesByIdAsync(List<Guid> summaryIds)
    {
        await _context.TicketSummaries
            .Where(a => summaryIds.Contains(a.Id))
            .ExecuteDeleteAsync();
    }
}