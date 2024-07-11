using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.DAL.Interfaces;
public interface ITicketNotificationRepository
{
    /// <summary>
    /// Get notifications by ticket id
    /// </summary>
    /// <param name="notifications"></param>
    /// <returns></returns>
    Task CreateManyNotificationsAsync(List<Notification> notifications);

    /// <summary>
    /// Create notification for ticket
    /// </summary>
    /// <param name="notification"></param>
    /// <returns></returns>
    Task CreateNotificationAsync(Notification notification);

    /// <summary>
    /// Delete many notifications for ticket
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    Task DeleteManyNotificationsAsync(List<Guid> ids);

    /// <summary>
    /// Delete notification for ticket
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteNotificationAsync(Guid id);

    /// <summary>
    /// Get notifications by ticket id
    /// </summary>
    /// <param name="ticketId"></param>
    /// <param name="paginationRequestDto"></param>
    /// <returns></returns>
    Task<PaginationResponseDto<Notification>> GetNotificationsByTicketIdAsync(
        Guid ticketId, PaginationRequestDto paginationRequestDto);

    /// <summary>
    /// Update many notifications for ticket
    /// </summary>
    /// <param name="notifications"></param>
    /// <returns></returns>
    Task UpdateManyNotificationsAsync(List<Notification> notifications);

    /// <summary>
    /// Update notification for ticket
    /// </summary>
    /// <param name="notification"></param>
    /// <returns></returns>
    Task UpdateNotificationAsync(Notification notification);
}