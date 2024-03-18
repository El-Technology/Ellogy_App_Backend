using TicketsManager.BLL.Dtos.TicketDtos;
using TicketsManager.Common.Dtos;

namespace TicketsManager.BLL.Interfaces;

/// <summary>
/// Interface for managing tickets related operations.
/// </summary>
public interface ITicketsService
{
    /// <summary>
    /// Retrieve a paginated list of tickets for the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose tickets are to be retrieved.</param>
    /// <param name="paginateRequest">The pagination configuration for the result.</param>
    /// <param name="userIdFromToken">User id taken from jwt token</param>
    /// <returns>Returns a paginated response containing the list of tickets for the user.</returns>
    Task<PaginationResponseDto<TicketResponseDto>> GetTicketsAsync(Guid userId, PaginationRequestDto paginateRequest, Guid userIdFromToken);

    /// <summary>
    /// Create a new ticket for the specified user.
    /// </summary>
    /// <param name="createTicketRequest">The data required to create the ticket.</param>
    /// <param name="userId">The unique identifier of the user creating the ticket.</param>
    /// <param name="userIdFromToken">User id taken from jwt token</param>
    /// <returns>Returns the created ticket details in the response.</returns>
    Task<TicketResponseDto> CreateTicketAsync(TicketCreateRequestDto createTicketRequest, Guid userId, Guid userIdFromToken);

    /// <summary>
    /// Delete a ticket with the given ID.
    /// </summary>
    /// <param name="id">The unique identifier of the ticket to be deleted.</param>
    /// <param name="userIdFromToken">User id taken from jwt token</param>
    /// <returns>Returns a task representing the asynchronous delete operation.</returns>
    Task DeleteTicketAsync(Guid id, Guid userIdFromToken);

    /// <summary>
    /// Update an existing ticket with the given ID.
    /// </summary>
    /// <param name="id">The unique identifier of the ticket to be updated.</param>
    /// <param name="ticketUpdate">The updated data for the ticket.</param>
    /// <param name="userIdFromToken">User id taken from jwt token</param>
    /// <returns>Returns the updated ticket details in the response.</returns>
    Task<TicketResponseDto> UpdateTicketAsync(Guid id, TicketUpdateRequestDto ticketUpdate, Guid userIdFromToken);

    /// <summary>
    /// Search for tickets belonging to the specified user based on the provided search criteria.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose tickets are to be searched.</param>
    /// <param name="searchRequest">The criteria for searching tickets by name.</param>
    /// <param name="userIdFromToken">User id taken from jwt token</param>
    /// <returns>Returns a paginated response containing the matching tickets for the user.</returns>
    Task<PaginationResponseDto<TicketResponseDto>> SearchTicketsByNameAsync(Guid userId, SearchTicketsRequestDto searchRequest, Guid userIdFromToken);

    /// <summary>
    /// Downloads the provided data as a DOC document asynchronously.
    /// </summary>
    /// <param name="base64Data">An array of base64-encoded data to be included in the DOC.</param>
    /// <returns>Returns the downloaded DOC document as a byte array.</returns>
    byte[] DownloadAsDoc(string[] base64Data);
}
