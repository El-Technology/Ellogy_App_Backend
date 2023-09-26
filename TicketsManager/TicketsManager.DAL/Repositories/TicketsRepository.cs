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
            .Include(e => e.Notifications)
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

    public async Task CheckTicketUpdateIds(Ticket ticket)
    {
        var messageIds = ticket.TicketMessages.Where(e => e.Id != Guid.Empty).Select(e => e.Id).ToList();
        foreach (var messageId in messageIds)
            if (!await _context.Messages.AnyAsync(e => e.Id == messageId))
                throw new Exception($"Message with id {messageId} was not found, we can`t update it.");

        var summaryIds = ticket.TicketSummaries.Where(e => e.Id != Guid.Empty).Select(e => e.Id).ToList();
        foreach (var summaryId in summaryIds)
            if (!await _context.TicketSummaries.AnyAsync(e => e.Id == summaryId))
                throw new Exception($"Summary with id {summaryId} was not found, we can`t update it.");

        var usecasesIds = ticket.Usecases.Where(e => e.Id != Guid.Empty).Select(e => e.Id).ToList();
        foreach (var usecaseId in usecasesIds)
            if (!await _context.Usecases.AnyAsync(e => e.Id == usecaseId))
                throw new Exception($"Usecase with id {usecaseId} was not found, we can`t update it.");

        var notificationIds = ticket.Notifications.Where(e => e.Id != Guid.Empty).Select(e => e.Id).ToList();
        foreach (var notificationId in notificationIds)
            if (!await _context.Notifications.AnyAsync(e => e.Id == notificationId))
                throw new Exception($"Notification with id {notificationId} was not found, we can`t update it.");
    }
}
