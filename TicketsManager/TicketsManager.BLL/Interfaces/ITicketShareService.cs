using TicketsManager.BLL.Dtos.TicketShareDtos;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.BLL.Interfaces;
public interface ITicketShareService
{
    /// <summary>
    /// Create a new ticket share
    /// </summary>
    /// <param name="ownerId"></param>
    /// <param name="createTicketShareDto"></param>
    /// <returns></returns>
    Task<TicketShare> CreateTicketShareAsync(
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
    /// <param name="paginationRequestDto"></param>
    /// <returns></returns>
    Task<PaginationResponseDto<TicketShare>> GetListOfSharesAsync(
        Guid ticketId, PaginationRequestDto paginationRequestDto);

    /// <summary>
    /// Update a ticket share
    /// </summary>
    /// <param name="ticketShare"></param>
    /// <returns></returns>
    Task UpdateTicketShareAsync(TicketShare ticketShare);
}