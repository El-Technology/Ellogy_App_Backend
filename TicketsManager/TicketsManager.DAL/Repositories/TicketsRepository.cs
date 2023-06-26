using Microsoft.EntityFrameworkCore;
using TicketsManager.Common.Helpers.Pagination;
using TicketsManager.DAL.Context;
using TicketsManager.DAL.Exceptions;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Repositories;

public class TicketsRepository : ITicketsRepository
{
    private readonly TicketsManagerDbContext _context;

    public TicketsRepository(TicketsManagerDbContext context)
    {
        _context = context;
    }

    public async Task<PaginationResponseDto<Ticket>> GetTicketsAsync(Guid userId, PaginationRequestDto paginateRequest)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(e => e.UserTickets)
            .ThenInclude(e => e.TicketMessages)
            .FirstOrDefaultAsync(e => e.Id == userId);

        return user is null
            ? throw new EntityNotFoundException(typeof(User))
            : user.UserTickets.GetPaginatedCollectionAsync(paginateRequest);
    }

    public async Task CreateTicketAsync(Ticket ticket)
    {
        await _context.AddAsync(ticket);
        await _context.SaveChangesAsync();
    }

    public Task<Ticket?> GetTicketByIdAsync(Guid id)
    {
        return _context.Tickets
            .Include(e => e.User)
            .Include(e => e.TicketMessages)
            .AsTracking()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task DeleteTicketAsync(Ticket ticket)
    {
        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTicketAsync(Ticket ticket)
    {
        _context.Tickets.Update(ticket);
        await _context.SaveChangesAsync();
    }

    public Task<bool> CheckIfTicketExistAsync(Guid id)
    {
        return _context.Tickets.AnyAsync(e => e.Id == id);
    }
}
