using Microsoft.EntityFrameworkCore;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Context;
using TicketsManager.DAL.Extensions;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.DAL.Repositories;
public class TicketNotificationRepository : ITicketNotificationRepository
{
    private readonly TicketsManagerDbContext _context;
    public TicketNotificationRepository(TicketsManagerDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc cref="ITicketNotificationRepository.GetNotificationsByTicketIdAsync" />
    public async Task<PaginationResponseDto<Notification>> GetNotificationsByTicketIdAsync(
        Guid ticketId, PaginationRequestDto paginationRequestDto)
    {
        return await _context.Notifications
            .Where(n => n.TicketId == ticketId)
            .GetFinalResultAsync(paginationRequestDto);
    }

    /// <inheritdoc cref="ITicketNotificationRepository.GetNotificationAsync" />
    public async Task<Notification?> GetNotificationAsync(Guid notificationId)
    {
        return await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId);
    }

    /// <inheritdoc cref="ITicketNotificationRepository.GetNotificationRangeAsync" />
    public async Task<IEnumerable<Notification>?> GetNotificationRangeAsync(
        List<Guid> guids)
    {
        return await _context.Notifications
            .Where(n => guids.Contains(n.Id))
            .ToListAsync();
    }

    /// <inheritdoc cref="ITicketNotificationRepository.CreateNotificationAsync" />
    public async Task CreateNotificationAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="ITicketNotificationRepository.CreateManyNotificationsAsync" />
    public async Task CreateManyNotificationsAsync(List<Notification> notifications)
    {
        await _context.Notifications.AddRangeAsync(notifications);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="ITicketNotificationRepository.UpdateNotificationAsync" />
    public async Task UpdateNotificationAsync(Notification notification)
    {
        _context.Notifications.Update(notification);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="ITicketNotificationRepository.UpdateManyNotificationsAsync" />
    public async Task UpdateManyNotificationsAsync(List<Notification> notifications)
    {
        _context.Notifications.UpdateRange(notifications);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="ITicketNotificationRepository.DeleteNotificationAsync" />
    public async Task DeleteNotificationAsync(Guid id)
    {
        await _context.Notifications
            .Where(a => a.Id == id)
            .ExecuteDeleteAsync();
    }

    /// <inheritdoc cref="ITicketNotificationRepository.DeleteManyNotificationsAsync" />
    public async Task DeleteManyNotificationsAsync(List<Guid> ids)
    {
        await _context.Notifications
            .Where(a => ids.Contains(a.Id))
            .ExecuteDeleteAsync();
    }
}
