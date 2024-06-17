using Microsoft.EntityFrameworkCore;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Context;
using TicketsManager.DAL.Extensions;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.DAL.Repositories;
public class TicketShareRepository : ITicketShareRepository
{
    private readonly TicketsManagerDbContext _context;

    public TicketShareRepository(TicketsManagerDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc cref="ITicketShareRepository.VerifyIfUserIsTicketOwnerAsync" />
    public async Task<bool> VerifyIfUserIsTicketOwnerAsync(Guid ownerId, Guid ticketId)
    {
        return await _context.Tickets
            .AnyAsync(e => e.UserId == ownerId && e.Id == ticketId);
    }

    /// <inheritdoc cref="ITicketShareRepository.CreateTicketShareAsync" />
    public async Task CreateTicketShareAsync(TicketShare ticketShare)
    {
        await _context.AddAsync(ticketShare);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="ITicketShareRepository.GetTicketSharesAsync" />
    public async Task<PaginationResponseDto<TicketShare>> GetTicketSharesAsync(
        Guid ticketId, PaginationRequestDto paginationRequestDto)
    {
        return await _context.TicketShares
            .Where(e => e.TicketId == ticketId)
            .GetFinalResultAsync(paginationRequestDto);
    }

    /// <inheritdoc cref="ITicketShareRepository.UpdateTicketShareAsync" />
    public async Task UpdateTicketShareAsync(TicketShare ticketShare)
    {
        _context.Update(ticketShare);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="ITicketShareRepository.DeleteTicketShareAsync" />
    public async Task DeleteTicketShareAsync(Guid ticketShareId)
    {
        var numberOfDeletedRows = await _context.TicketShares
            .Where(ts => ts.Id == ticketShareId)
            .ExecuteDeleteAsync();

        if (numberOfDeletedRows != 1)
            throw new InvalidOperationException("Failed to delete ticket share");
    }
}
