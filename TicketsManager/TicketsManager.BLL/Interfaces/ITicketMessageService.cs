using TicketsManager.BLL.Dtos.MessageDtos;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Enums;

namespace TicketsManager.BLL.Interfaces;
public interface ITicketMessageService
{
    /// <summary>
    /// Create message
    /// </summary>
    /// <param name="messageDto"></param>
    /// <param name="ticketId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<MessageResponseDto> CreateMessageAsync(
        MessageDto messageDto, Guid ticketId, Guid userId);

    /// <summary>
    /// Create range messages
    /// </summary>
    /// <param name="messageDtos"></param>
    /// <param name="ticketId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<IEnumerable<MessageResponseDto>> CreateRangeMessagesAsync(
        List<MessageDto> messageDtos, Guid ticketId, Guid userId);

    /// <summary>
    /// Delete message
    /// </summary>
    /// <param name="messageId"></param>
    /// <param name="userId"></param>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    Task DeleteMessageAsync(Guid messageId, Guid userId, Guid ticketId);

    /// <summary>
    /// Delete range messages
    /// </summary>
    /// <param name="messageIds"></param>
    /// <param name="userId"></param>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    Task DeleteRangeMessagesAsync(List<Guid> messageIds, Guid userId, Guid ticketId);

    /// <summary>
    /// Get ticket messages by ticket id
    /// </summary>
    /// <param name="ticketId"></param>
    /// <param name="userId"></param>
    /// <param name="paginationRequest"></param>
    /// <param name="subStageEnum"></param>
    /// <returns></returns>
    Task<PaginationResponseDto<MessageResponseDto>> GetTicketMessagesByTicketIdAsync(
        Guid ticketId,
        Guid userId,
        PaginationRequestDto paginationRequest,
        SubStageEnum? subStageEnum);

    /// <summary>
    /// Update message
    /// </summary>
    /// <param name="messageRequestDto"></param>
    /// <param name="ticketId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<MessageResponseDto> UpdateMessageAsync(
        MessageResponseDto messageRequestDto, Guid ticketId, Guid userId);

    /// <summary>
    /// Update range messages
    /// </summary>
    /// <param name="messageResponseDtos"></param>
    /// <param name="ticketId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<IEnumerable<MessageResponseDto>> UpdateRangeMessagesAsync(
        List<MessageResponseDto> messageResponseDtos, Guid ticketId, Guid userId);
}