using TicketsManager.BLL.Dtos.TicketShareDtos;
using TicketsManager.Common.Dtos;

namespace TicketsManager.BLL.Interfaces;
public interface ITicketShareService
{
    /// <summary>
    /// Create a new ticket share
    /// </summary>
    /// <param name="ownerId"></param>
    /// <param name="createTicketShareDto"></param>
    /// <returns></returns>
    Task CreateTicketShareAsync(
        Guid ownerId, CreateTicketShareDto createTicketShareDto);

    /// <summary>
    /// Delete a ticket share
    /// </summary>
    /// <param name="ownerId"></param>
    /// <param name="ticketShareId"></param>
    /// <returns></returns>
    Task DeleteTicketShareAsync(
        Guid ownerId, Guid ticketShareId);

    /// <summary>
    /// Get ticket shares by ticket id using pagination
    /// </summary>
    /// <param name="ticketId"></param>
    /// <param name="ownerId"></param>
    /// <param name="paginationRequestDto"></param>
    /// <returns></returns>
    Task<PaginationResponseDto<GetTicketShareDto>> GetListOfSharesAsync(
        Guid ownerId, Guid ticketId, PaginationRequestDto paginationRequestDto);

    /// <summary>
    /// Update a ticket share
    /// </summary>
    /// <param name="ticketShare"></param>
    /// <param name="ticketShareId"></param>
    /// <param name="ownerId"></param>
    /// <returns></returns>
    Task UpdateTicketShareAsync(Guid ownerId, Guid ticketShareId, UpdateTicketShareDto ticketShare);
}