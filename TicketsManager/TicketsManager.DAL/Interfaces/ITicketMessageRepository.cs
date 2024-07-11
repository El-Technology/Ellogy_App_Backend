using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.DAL.Interfaces;
public interface ITicketMessageRepository
{
    /// <summary>
    /// Create message
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task CreateMessageAsync(Message message);

    /// <summary>
    /// Create range messages
    /// </summary>
    /// <param name="messages"></param>
    /// <returns></returns>
    Task CreateRangeMessagesAsync(List<Message> messages);

    /// <summary>
    /// Delete message
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task DeleteMessageAsync(Message message);

    /// <summary>
    /// Delete range messages
    /// </summary>
    /// <param name="messages"></param>
    /// <returns></returns>
    Task DeleteRangeMessagesAsync(List<Message> messages);

    /// <summary>
    /// Get message by id
    /// </summary>
    /// <param name="messageId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Message?> GetMessageAsync(Guid messageId, Guid userId);

    /// <summary>
    /// Get range messages by ids
    /// </summary>
    /// <param name="messageId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<Message>?> GetRangeMessagesAsync(List<Guid> messageId, Guid userId);

    /// <summary>
    /// Get ticket messages by ticket id
    /// </summary>
    /// <param name="ticketId"></param>
    /// <param name="userId"></param>
    /// <param name="paginationRequest"></param>
    /// <param name="subStageEnum"></param>
    /// <returns></returns>
    Task<PaginationResponseDto<Message>> GetTicketMessagesByTicketIdAsync(
        Guid ticketId, Guid userId, PaginationRequestDto paginationRequest, SubStageEnum? subStageEnum);

    /// <summary>
    /// Update message
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task UpdateMessageAsync(Message message);

    /// <summary>
    /// Update range messages
    /// </summary>
    /// <param name="messages"></param>
    /// <returns></returns>
    Task UpdateRangeMessagesAsync(List<Message> messages);
}