using Microsoft.EntityFrameworkCore;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Context;
using TicketsManager.DAL.Extensions;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.TicketSummaryModels;
using TicketsManager.DAL.Models.UsecaseModels;

namespace TicketsManager.DAL.Repositories;

public class UsecaseRepository : IUsecaseRepository
{
    private readonly TicketsManagerDbContext _context;

    public UsecaseRepository(TicketsManagerDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetLastOrderForUsecaseByTicketIdAsync(Guid ticketId)
    {
        return await _context.Usecases
            .Where(a => a.TicketId == ticketId)
            .Select(a => a.Order)
            .DefaultIfEmpty()
            .MaxAsync();
    }

    public IQueryable<TicketSummary> GetTicketSummariesByIdsAsync(List<Guid> ticketSummaryIds)
    {
        return _context.TicketSummaries.Where(a => ticketSummaryIds.Contains(a.Id));
    }

    public async Task<Guid> GetUserIdByTicketIdAsync(Guid ticketId)
    {
        return await _context.Tickets.Where(a => a.Id == ticketId).Select(a => a.UserId).FirstOrDefaultAsync();
    }

    /// <inheritdoc cref="IUsecaseRepository.CreateUsecasesAsync(List{Usecase})" />
    public async Task CreateUsecasesAsync(List<Usecase> usecases)
    {
        var allTicketSummaryIds = usecases.SelectMany(u => u.TicketSummaries.Select(ts => ts.Id)).ToList();

        var allTicketSummaries = _context.TicketSummaries
            .Where(ts => allTicketSummaryIds.Contains(ts.Id))
            .ToList();

        foreach (var item in usecases)
        {
            var itemTicketSummaryIds = item.TicketSummaries.Select(ts => ts.Id).ToList();
            item.TicketSummaries = allTicketSummaries
                .Where(ts => itemTicketSummaryIds.Contains(ts.Id))
                .ToList();
        }

        await _context.Usecases.AddRangeAsync(usecases);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="IUsecaseRepository.GetUsecasesAsync(PaginationRequestDto, Guid)" />
    public async Task<PaginationResponseDto<Usecase>> GetUsecasesAsync(
        PaginationRequestDto paginationRequest,
        Guid ticketId)
    {
        return await _context.Usecases
            .Where(a => a.TicketId == ticketId)
            .OrderBy(a => a.Order)
            .Include(a => a.Tables)
            .Include(a => a.Diagrams)
            .Include(a => a.TicketSummaries)
            .GetFinalResultAsync(paginationRequest);
    }

    /// <inheritdoc cref="IUsecaseRepository.UpdateUsecaseAsync(Usecase)" />
    public async Task UpdateUsecaseAsync(Usecase usecase)
    {
        _context.Update(usecase);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="IUsecaseRepository.GetUsecaseByIdAsync(Guid)" />
    public Task<Usecase?> GetUsecaseByIdAsync(Guid usecaseId)
    {
        return _context.Usecases
            .AsTracking()
            .FirstOrDefaultAsync(a => a.Id == usecaseId);
    }

    /// <inheritdoc cref="IUsecaseRepository.DeleteUsecasesAsync(Guid)" />
    public async Task DeleteUsecasesAsync(Guid ticketId)
    {
        await _context.Usecases
            .Where(a => a.TicketId == ticketId)
            .ExecuteDeleteAsync();
    }
}