using Microsoft.EntityFrameworkCore;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Context;
using TicketsManager.DAL.Extensions;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.DAL.Repositories;

public class TicketsRepository : ITicketsRepository
{
    private readonly TicketsManagerDbContext _context;

    public TicketsRepository(TicketsManagerDbContext context)
    {
        _context = context;
    }

    private IQueryable<Ticket> GetTicketsByUserIdQuery(Guid userId)
    {
        return _context.Tickets
            .Include(e => e.TicketShares.Where(a => a.SharedUserId == userId))
            .Include(e => e.TicketMessages
                .Where(msg =>
                    msg.Ticket.UserId == userId ||
                    msg.Ticket.TicketShares.Any(ts =>
                        ts.SharedUserId == userId &&
                        (
                            ts.TicketCurrentStep == Enums.TicketCurrentStepEnum.General ||
                            ts.TicketCurrentStep == null ||
                            ts.TicketCurrentStep == Enums.TicketCurrentStepEnum.Report
                        ) &&
                        (
                            ts.TicketCurrentStep == null ||
                            ts.SubStageEnum == null ||
                            ts.SubStageEnum == msg.SubStage ||
                            msg.SubStage == Enums.SubStageEnum.FunctionalRequirements
                        )
                    )
                )
                .OrderBy(a => a.SendTime)
            )
            .Include(e => e.Notifications
                .Where(notification =>
                    notification.Ticket.UserId == userId ||
                    notification.Ticket.TicketShares.Any(ts =>
                        ts.SharedUserId == userId &&
                        (
                            ts.TicketCurrentStep == null ||
                            ts.TicketCurrentStep == Enums.TicketCurrentStepEnum.Notifications ||
                            ts.TicketCurrentStep == Enums.TicketCurrentStepEnum.Report
                        )
                    )
                )
            )
            .AsNoTracking()
            .Where(a =>
                a.UserId == userId ||
                a.TicketShares.Any(ts =>
                    ts.SharedUserId == userId &&
                    (ts.RevokedAt > DateTime.UtcNow || ts.RevokedAt == null))
            );
    }

    /// <inheritdoc cref="ITicketsRepository.GetTicketsAsync" />
    public async Task<PaginationResponseDto<Ticket>> GetTicketsAsync(
        Guid userId, PaginationRequestDto paginateRequest)
    {
        return await GetTicketsByUserIdQuery(userId)
            .GetFinalResultAsync(paginateRequest);
    }

    /// <inheritdoc cref="ITicketsRepository.FindTicketsAsync" />
    public async Task<PaginationResponseDto<Ticket>> FindTicketsAsync(Guid userId,
        SearchTicketsRequestDto searchTicketsRequest)
    {
        return await GetTicketsByUserIdQuery(userId)
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
            .AsNoTracking()
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
        if (messageIds.Any())
        {
            var existingMessageIds = await _context.Messages
                .Where(e => messageIds.Contains(e.Id))
                .Select(e => e.Id)
                .ToListAsync();

            var missingMessageIds = messageIds.Except(existingMessageIds).ToList();
            if (missingMessageIds.Any())
                throw new Exception($"Messages with ids {string.Join(", ", missingMessageIds)} were not found, we can't update them.");
        }

        var notificationIds = ticket.Notifications.Where(e => e.Id != Guid.Empty).Select(e => e.Id).ToList();
        if (notificationIds.Any())
        {
            var existingNotificationIds = await _context.Notifications
                .Where(e => notificationIds.Contains(e.Id))
                .Select(e => e.Id)
                .ToListAsync();

            var missingNotificationIds = notificationIds.Except(existingNotificationIds).ToList();
            if (missingNotificationIds.Any())
                throw new Exception($"Notifications with ids {string.Join(", ", missingNotificationIds)} were not found, we can't update them.");
        }
    }
}