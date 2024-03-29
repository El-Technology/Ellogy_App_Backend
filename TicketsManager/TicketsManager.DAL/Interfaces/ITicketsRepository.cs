using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Interfaces;

public interface ITicketsRepository
{
    /// <summary>
    /// Get all tickets for a user with a specific id
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="paginateRequest"></param>
    /// <returns></returns>
    public Task<PaginationResponseDto<Ticket>> GetTicketsAsync(
        Guid userId, PaginationRequestDto paginateRequest);

    /// <summary>
    /// Find tickets for a user with a specific id
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="searchTicketsRequest"></param>
    /// <returns></returns>
    public Task<PaginationResponseDto<Ticket>> FindTicketsAsync(
        Guid userId, SearchTicketsRequestDto searchTicketsRequest);

    /// <summary>
    /// Create a new ticket
    /// </summary>
    /// <param name="ticket"></param>
    /// <returns></returns>
    public Task CreateTicketAsync(Ticket ticket);

    /// <summary>
    /// Get a ticket by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<Ticket?> GetTicketByIdAsync(Guid id);

    /// <summary>
    /// Update a ticket
    /// </summary>
    /// <param name="ticket"></param>
    /// <returns></returns>
    public Task UpdateTicketAsync(Ticket ticket);

    /// <summary>
    /// Delete a ticket by id
    /// </summary>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    public Task DeleteTicketAsync(Guid ticketId);

    /// <summary>
    /// Check if a ticket exists
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<bool> CheckIfTicketExistAsync(Guid id);

    /// <summary>
    /// Check if a ticket has been updated
    /// </summary>
    /// <param name="ticket"></param>
    /// <returns></returns>
    Task CheckTicketUpdateIds(Ticket ticket);
}
