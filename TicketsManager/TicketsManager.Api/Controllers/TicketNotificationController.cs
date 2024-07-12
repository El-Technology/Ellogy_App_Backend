using Microsoft.AspNetCore.Mvc;
using TicketsManager.BLL.Dtos.NotificationDtos;
using TicketsManager.BLL.Interfaces;
using TicketsManager.Common.Dtos;
using TicketsManager.Common.Helpers;

namespace TicketsManager.Api.Controllers;

/// <summary>
/// Controller for ticket notification
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class TicketNotificationController : ControllerBase
{
    private readonly ITicketNotificationService _ticketNotificationService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="ticketNotificationService"></param>
    public TicketNotificationController(ITicketNotificationService ticketNotificationService)
    {
        _ticketNotificationService = ticketNotificationService;
    }

    /// <summary>
    /// This method retrieves the user id from the JWT token
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private Guid GetUserIdFromToken() =>
        TokenParseHelper.GetUserId(User);

    /// <summary>
    /// Create notification for ticket
    /// </summary>
    /// <param name="ticketId"></param>
    /// <param name="paginationRequestDto"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(PaginationResponseDto<NotificationFullDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    [HttpPost]
    [Route("getNotifications")]
    public async Task<IActionResult> GetNotificationsByTicketIdAsync(
        [FromQuery] Guid ticketId,
        [FromBody] PaginationRequestDto paginationRequestDto)
    {
        var notifications = await _ticketNotificationService
            .GetNotificationsByTicketIdAsync(ticketId, GetUserIdFromToken(), paginationRequestDto);

        return Ok(notifications);
    }

    /// <summary>
    /// Create notification for ticket
    /// </summary>
    /// <param name="notificationCreateDto"></param>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(NotificationFullDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    [HttpPost]
    [Route("createNotification")]
    public async Task<IActionResult> CreateNotificationAsync(
        [FromBody] NotificationDto notificationCreateDto,
        [FromQuery] Guid ticketId)
    {
        var notification = await _ticketNotificationService
            .CreateNotificationAsync(notificationCreateDto, ticketId, GetUserIdFromToken());

        return Ok(notification);
    }

    /// <summary>
    /// Create many notifications for ticket
    /// </summary>
    /// <param name="notificationsCreateDto"></param>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(NotificationFullDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    [HttpPost]
    [Route("createManyNotifications")]
    public async Task<IActionResult> CreateManyNotificationsAsync(
        [FromBody] List<NotificationDto> notificationsCreateDto,
        [FromQuery] Guid ticketId)
    {
        var notifications = await _ticketNotificationService
            .CreateManyNotificationsAsync(notificationsCreateDto, ticketId, GetUserIdFromToken());

        return Ok(notifications);
    }

    /// <summary>
    /// Update notification for ticket
    /// </summary>
    /// <param name="notificationUpdateDto"></param>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(NotificationFullDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    [HttpPut]
    [Route("updateNotification")]
    public async Task<IActionResult> UpdateNotificationAsync(
        [FromBody] NotificationFullDto notificationUpdateDto,
        [FromQuery] Guid ticketId)
    {
        var notification = await _ticketNotificationService
            .UpdateNotificationAsync(notificationUpdateDto, ticketId, GetUserIdFromToken());

        return Ok(notification);
    }

    /// <summary>
    /// Update many notifications for ticket
    /// </summary>
    /// <param name="notificationsUpdateDto"></param>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(NotificationFullDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    [HttpPut]
    [Route("updateManyNotifications")]
    public async Task<IActionResult> UpdateManyNotificationsAsync(
        [FromBody] List<NotificationFullDto> notificationsUpdateDto,
        [FromQuery] Guid ticketId)
    {
        var notifications = await _ticketNotificationService
            .UpdateManyNotificationsAsync(notificationsUpdateDto, ticketId, GetUserIdFromToken());

        return Ok(notifications);
    }

    /// <summary>
    /// Delete notification for ticket
    /// </summary>
    /// <param name="notificationId"></param>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    [HttpDelete]
    [Route("deleteNotification")]
    public async Task<IActionResult> DeleteNotificationAsync(
        [FromQuery] Guid notificationId,
        [FromQuery] Guid ticketId)
    {
        await _ticketNotificationService.DeleteNotificationAsync(
            notificationId, ticketId, GetUserIdFromToken());

        return Ok();
    }

    /// <summary>
    /// Delete many notifications for ticket
    /// </summary>
    /// <param name="notificationIds"></param>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    [HttpDelete]
    [Route("deleteManyNotifications")]
    public async Task<IActionResult> DeleteManyNotificationsAsync(
        [FromBody] List<Guid> notificationIds,
        [FromQuery] Guid ticketId)
    {
        await _ticketNotificationService.DeleteManyNotificationsAsync(
            notificationIds, ticketId, GetUserIdFromToken());

        return Ok();
    }
}
