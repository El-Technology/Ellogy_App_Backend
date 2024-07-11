using TicketsManager.BLL.Dtos.NotificationDtos;
using TicketsManager.Common.Dtos;

namespace TicketsManager.BLL.Interfaces;
public interface ITicketNotificationService
{
    /// <summary>
    /// Create notification for ticket
    /// </summary>
    /// <param name="notificationsCreateDto"></param>
    /// <param name="ticketId"></param>
    /// <param name="userIdFromToken"></param>
    /// <returns></returns>
    Task<IEnumerable<NotificationFullDto>> CreateManyNotificationsAsync(
        List<NotificationDto> notificationsCreateDto, Guid ticketId, Guid userIdFromToken);

    /// <summary>
    /// Create notification for ticket
    /// </summary>
    /// <param name="notificationCreateDto"></param>
    /// <param name="ticketId"></param>
    /// <param name="userIdFromToken"></param>
    /// <returns></returns>
    Task<NotificationFullDto> CreateNotificationAsync(
        NotificationDto notificationCreateDto, Guid ticketId, Guid userIdFromToken);

    /// <summary>
    /// Delete many notifications for ticket
    /// </summary>
    /// <param name="notificationIds"></param>
    /// <param name="ticketId"></param>
    /// <param name="userIdFromToken"></param>
    /// <returns></returns>
    Task DeleteManyNotificationsAsync(
        List<Guid> notificationIds, Guid ticketId, Guid userIdFromToken);

    /// <summary>
    /// Delete notification for ticket
    /// </summary>
    /// <param name="notificationId"></param>
    /// <param name="ticketId"></param>
    /// <param name="userIdFromToken"></param>
    /// <returns></returns>
    Task DeleteNotificationAsync(
        Guid notificationId, Guid ticketId, Guid userIdFromToken);

    /// <summary>
    /// Get notifications by ticket id
    /// </summary>
    /// <param name="ticketId"></param>
    /// <param name="userIdFromToken"></param>
    /// <param name="paginationRequestDto"></param>
    /// <returns></returns>
    Task<PaginationResponseDto<NotificationFullDto>> GetNotificationsByTicketIdAsync(
        Guid ticketId, Guid userIdFromToken, PaginationRequestDto paginationRequestDto);

    /// <summary>
    /// Update many notifications for ticket
    /// </summary>
    /// <param name="notificationsUpdateDto"></param>
    /// <param name="ticketId"></param>
    /// <param name="userIdFromToken"></param>
    /// <returns></returns>
    Task<IEnumerable<NotificationFullDto>> UpdateManyNotificationsAsync(
        List<NotificationFullDto> notificationsUpdateDto, Guid ticketId, Guid userIdFromToken);

    /// <summary>
    /// Update notification for ticket
    /// </summary>
    /// <param name="notificationUpdateDto"></param>
    /// <param name="ticketId"></param>
    /// <param name="userIdFromToken"></param>
    /// <returns></returns>
    Task<NotificationFullDto> UpdateNotificationAsync(
        NotificationFullDto notificationUpdateDto, Guid ticketId, Guid userIdFromToken);
}