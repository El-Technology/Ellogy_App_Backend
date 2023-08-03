using Microsoft.EntityFrameworkCore;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Context;
using TicketsManager.DAL.Extensions;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Repositories;

public class TicketsRepository : ITicketsRepository
{
    private readonly TicketsManagerDbContext _context;
    private readonly IUserRepository _userRepository;

    public TicketsRepository(TicketsManagerDbContext context, IUserRepository userRepository)
    {
        _context = context;
        _userRepository = userRepository;
    }

    public async Task<PaginationResponseDto<Ticket>> GetTicketsAsync(Guid userId, PaginationRequestDto paginateRequest)
    {
        var user = await _userRepository.GetUserAsync(userId);

        return user.UserTickets.GetFinalResult(paginateRequest);
    }

    public async Task<PaginationResponseDto<Ticket>> FindTicketsAsync(Guid userId, SearchTicketsRequestDto searchTicketsRequest)
    {
        var user = await _userRepository.GetUserAsync(userId);

        return user.UserTickets
            .Where(e => e.Title.Contains(searchTicketsRequest.TicketTitle, StringComparison.InvariantCultureIgnoreCase))
            .GetFinalResult(searchTicketsRequest.Pagination);
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
            .Include(e => e.TicketSummaries)
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
