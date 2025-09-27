using Microsoft.EntityFrameworkCore;
using TicketsManager.DAL.Context;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.DAL.Repositories;

public class TraceabilityRepository : ITraceabilityRepository
{
    private readonly TicketsManagerDbContext _context;

    public TraceabilityRepository(TicketsManagerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TraceabilityMatrixView>> GetTraceabilityMatrixViewAsync(Guid ticketId)
    {
        // Since we're using a database view, we can query it directly
        // The view should be mapped in the DbContext
        return await _context.Set<TraceabilityMatrixView>()
            .Where(tm => tm.TicketId == ticketId)
            .OrderBy(tm => tm.UserStoryId)
            .ThenBy(tm => tm.ScenarioId)
            .ThenBy(tm => tm.AcceptanceId)
            .ThenBy(tm => tm.UserStoryTestId)
            .ThenBy(tm => tm.TestCaseId)
            .ToListAsync();
    }
}