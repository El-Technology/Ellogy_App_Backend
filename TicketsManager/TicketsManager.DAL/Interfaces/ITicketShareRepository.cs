using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.DAL.Interfaces;
public interface ITicketShareRepository
{
    Task CheckIfUserHaveAccessToComponentByTicketIdAsync(
        Guid ticketId, Guid userId, TicketCurrentStepEnum currentStepEnum, SharePermissionEnum requireSharePermissionEnum);
    Task CheckIfUserHaveAccessToSubStageByTicketIdAsync(
        Guid ticketId, Guid userId, SubStageEnum? subStageEnum, SharePermissionEnum requireSharePermissionEnum);
    Task CreateManyTicketSharesAsync(List<TicketShare> ticketShares);

    /// <summary>
    /// Create a new ticket share
    /// </summary>
    /// <param name="ticketShare"></param>
    /// <returns></returns>
    Task CreateTicketShareAsync(TicketShare ticketShare);

    /// <summary>
    /// Delete a ticket share
    /// </summary>
    /// <param name="ticketShareId"></param>
    /// <returns></returns>
    Task DeleteTicketShareAsync(Guid ticketShareId);

    /// <summary>
    /// Get ticket id by ticket share id
    /// </summary>
    /// <param name="ticketShareId"></param>
    /// <returns></returns>
    Task<Guid> GetTicketIdByTicketShareIdAsync(Guid ticketShareId);

    /// <summary>
    /// Get ticket shares by ticket id using pagination
    /// </summary>
    /// <param name="ticketId"></param>
    /// <param name="paginationRequestDto"></param>
    /// <returns></returns>
    Task<PaginationResponseDto<TicketShare>> GetTicketSharesAsync(
        Guid ticketId, PaginationRequestDto paginationRequestDto);
    Task<string?> GetTicketTitleByTicketIdAsync(Guid ticketId);

    /// <summary>
    /// Verify if a permission is already given to a user
    /// </summary>
    /// <param name="ticketId"></param>
    /// <param name="sharedUserId"></param>
    /// <returns></returns>
    Task<bool> IfPermissionAlreadyGivenToUserAsync(Guid ticketId, Guid sharedUserId);

    /// <summary>
    /// Update a ticket share
    /// </summary>
    /// <param name="ticketShare"></param>
    /// <returns></returns>
    Task UpdateTicketShareAsync(Guid ticketShareId, TicketShare ticketShare);

    /// <summary>
    /// Verify if a user is the owner of a ticket
    /// </summary>
    /// <param name="ownerId"></param>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    Task<bool> VerifyIfUserIsTicketOwnerAsync(Guid ownerId, Guid ticketId);
}