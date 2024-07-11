using AutoMapper;
using TicketsManager.BLL.Dtos.NotificationDtos;
using TicketsManager.BLL.Interfaces;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.BLL.Services;
public class TicketNotificationService : ITicketNotificationService
{
    private readonly ITicketNotificationRepository _ticketNotificationRepository;
    private readonly IMapper _mapper;
    private readonly ITicketShareRepository _ticketShareRepository;

    public TicketNotificationService(
        ITicketNotificationRepository ticketNotificationRepository,
        IMapper mapper,
        ITicketShareRepository ticketShareRepository)
    {
        _ticketNotificationRepository = ticketNotificationRepository;
        _mapper = mapper;
        _ticketShareRepository = ticketShareRepository;
    }

    private async Task ValidateUserPermissionAsync(
        Guid ticketId,
        Guid userIdFromToken,
        SharePermissionEnum sharePermissionEnum)
    {
        await _ticketShareRepository.CheckIfUserHaveAccessToComponentByTicketIdAsync(
            ticketId,
            userIdFromToken,
            TicketCurrentStepEnum.Notifications,
            sharePermissionEnum);
    }

    /// <inheritdoc cref="ITicketNotificationService.GetNotificationsByTicketIdAsync" />
    public async Task<PaginationResponseDto<NotificationFullDto>> GetNotificationsByTicketIdAsync(
               Guid ticketId, Guid userIdFromToken, PaginationRequestDto paginationRequestDto)
    {
        await ValidateUserPermissionAsync(ticketId, userIdFromToken, SharePermissionEnum.Read);

        var notifications = await _ticketNotificationRepository
            .GetNotificationsByTicketIdAsync(ticketId, paginationRequestDto);

        return _mapper.Map<PaginationResponseDto<NotificationFullDto>>(notifications);
    }

    /// <inheritdoc cref="ITicketNotificationService.CreateNotificationAsync" />
    public async Task<NotificationFullDto> CreateNotificationAsync(
        NotificationDto notificationCreateDto, Guid ticketId, Guid userIdFromToken)
    {
        await ValidateUserPermissionAsync(ticketId, userIdFromToken, SharePermissionEnum.ReadWrite);

        var notification = _mapper.Map<Notification>(notificationCreateDto);
        notification.TicketId = ticketId;

        await _ticketNotificationRepository.CreateNotificationAsync(notification);

        return _mapper.Map<NotificationFullDto>(notification);
    }

    /// <inheritdoc cref="ITicketNotificationService.CreateManyNotificationsAsync" />
    public async Task<IEnumerable<NotificationFullDto>> CreateManyNotificationsAsync(
               List<NotificationDto> notificationsCreateDto, Guid ticketId, Guid userIdFromToken)
    {
        await ValidateUserPermissionAsync(ticketId, userIdFromToken, SharePermissionEnum.ReadWrite);

        var notifications = _mapper.Map<List<Notification>>(notificationsCreateDto);
        notifications.ForEach(n => n.TicketId = ticketId);

        await _ticketNotificationRepository.CreateManyNotificationsAsync(notifications);

        return _mapper.Map<IEnumerable<NotificationFullDto>>(notifications);
    }

    /// <inheritdoc cref="ITicketNotificationService.UpdateNotificationAsync" />
    public async Task<NotificationFullDto> UpdateNotificationAsync(
               NotificationFullDto notificationUpdateDto, Guid ticketId, Guid userIdFromToken)
    {
        await ValidateUserPermissionAsync(ticketId, userIdFromToken, SharePermissionEnum.ReadWrite);

        var notification = _mapper.Map<Notification>(notificationUpdateDto);
        notification.TicketId = ticketId;

        await _ticketNotificationRepository.UpdateNotificationAsync(notification);

        return _mapper.Map<NotificationFullDto>(notification);
    }

    /// <inheritdoc cref="ITicketNotificationService.UpdateManyNotificationsAsync" />
    public async Task<IEnumerable<NotificationFullDto>> UpdateManyNotificationsAsync(
                      List<NotificationFullDto> notificationsUpdateDto, Guid ticketId, Guid userIdFromToken)
    {
        await ValidateUserPermissionAsync(ticketId, userIdFromToken, SharePermissionEnum.ReadWrite);

        var notifications = _mapper.Map<List<Notification>>(notificationsUpdateDto);
        notifications.ForEach(n => n.TicketId = ticketId);

        await _ticketNotificationRepository.UpdateManyNotificationsAsync(notifications);

        return _mapper.Map<IEnumerable<NotificationFullDto>>(notifications);
    }

    /// <inheritdoc cref="ITicketNotificationService.DeleteNotificationAsync" />
    public async Task DeleteNotificationAsync(Guid notificationId, Guid ticketId, Guid userIdFromToken)
    {
        await ValidateUserPermissionAsync(ticketId, userIdFromToken, SharePermissionEnum.Manage);

        await _ticketNotificationRepository.DeleteNotificationAsync(notificationId);
    }

    /// <inheritdoc cref="ITicketNotificationService.DeleteManyNotificationsAsync" />
    public async Task DeleteManyNotificationsAsync(List<Guid> notificationIds, Guid ticketId, Guid userIdFromToken)
    {
        await ValidateUserPermissionAsync(ticketId, userIdFromToken, SharePermissionEnum.Manage);

        await _ticketNotificationRepository.DeleteManyNotificationsAsync(notificationIds);
    }
}
