using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Context;
using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Extensions;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.DAL.Repositories;

public class ActionHistoryRepository : IActionHistoryRepository
{
    private readonly TicketsManagerDbContext _context;
    public ActionHistoryRepository(TicketsManagerDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc cref="IActionHistoryRepository.CreateActionHistoryAsync" />
    public async Task CreateActionHistoryAsync(ActionHistory actionHistory)
    {
        await _context.ActionHistories.AddAsync(actionHistory);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="IActionHistoryRepository.GetActionHistoriesAsync" />
    public async Task<PaginationResponseDto<ActionHistory>> GetActionHistoriesAsync(
        Guid ticketId, TicketCurrentStepEnum ticketCurrentStepEnum, PaginationRequestDto paginationRequest)
    {
        return await _context.ActionHistories
            .Where(a => a.TicketId == ticketId)
            .Where(a => a.TicketCurrentStepEnum == ticketCurrentStepEnum)
            .GetFinalResultAsync(paginationRequest);
    }
}
