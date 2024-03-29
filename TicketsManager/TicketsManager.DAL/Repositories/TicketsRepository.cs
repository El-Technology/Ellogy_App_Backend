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

    public TicketsRepository(TicketsManagerDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc cref="ITicketsRepository.GetTicketsAsync" />
    public async Task<PaginationResponseDto<Ticket>> GetTicketsAsync(
        Guid userId, PaginationRequestDto paginateRequest)
    {
        return await _context.Tickets
            .Include(e => e.TicketMessages.OrderBy(e => e.SendTime))
            .Include(e => e.TicketSummaries)
            .Include(e => e.Notifications)
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .GetFinalResultAsync(paginateRequest);
    }

    /// <inheritdoc cref="ITicketsRepository.FindTicketsAsync" />
    public async Task<PaginationResponseDto<Ticket>> FindTicketsAsync(Guid userId,
        SearchTicketsRequestDto searchTicketsRequest)
    {
        return await _context.Tickets
            .Include(e => e.TicketMessages.OrderBy(e => e.SendTime))
            .Include(e => e.TicketSummaries)
            .Include(e => e.Notifications)
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .Where(e => e.Title.ToLower().Contains(searchTicketsRequest.TicketTitle.ToLower()))
            .GetFinalResultAsync(searchTicketsRequest.Pagination);
    }

    /// <inheritdoc cref="ITicketsRepository.CreateTicketAsync" />
    public async Task CreateTicketAsync(Ticket ticket)
    {
        await _context.AddAsync(ticket);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="ITicketsRepository.GetTicketByIdAsync" />
    public Task<Ticket?> GetTicketByIdAsync(Guid id)
    {
        return _context.Tickets
            .Include(e => e.TicketMessages)
            .Include(e => e.Notifications)
            .AsTracking()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    /// <inheritdoc cref="ITicketsRepository.DeleteTicketAsync" />
    public async Task DeleteTicketAsync(Guid ticketId)
    {
        await _context.Tickets
            .Where(a => a.Id == ticketId)
            .ExecuteDeleteAsync();
    }

    /// <inheritdoc cref="ITicketsRepository.UpdateTicketAsync" />
    public async Task UpdateTicketAsync(Ticket ticket)
    {
        _context.Tickets.Update(ticket);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="ITicketsRepository.CheckIfTicketExistAsync" />
    public Task<bool> CheckIfTicketExistAsync(Guid id)
    {
        return _context.Tickets.AnyAsync(e => e.Id == id);
    }

    /// <inheritdoc cref="ITicketsRepository.CheckTicketUpdateIds" />
    public async Task CheckTicketUpdateIds(Ticket ticket)
    {
        var messageIds = ticket.TicketMessages.Where(e => e.Id != Guid.Empty).Select(e => e.Id).ToList();
        foreach (var messageId in messageIds)
            if (!await _context.Messages.AnyAsync(e => e.Id == messageId))
                throw new Exception($"Message with id {messageId} was not found, we can`t update it.");

        var notificationIds = ticket.Notifications.Where(e => e.Id != Guid.Empty).Select(e => e.Id).ToList();
        foreach (var notificationId in notificationIds)
            if (!await _context.Notifications.AnyAsync(e => e.Id == notificationId))
                throw new Exception($"Notification with id {notificationId} was not found, we can`t update it.");
    }
}