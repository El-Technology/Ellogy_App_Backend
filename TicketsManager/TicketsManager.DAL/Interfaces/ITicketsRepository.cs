using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Interfaces;

public interface ITicketsRepository
{
    public Task<PaginationResponseDto<Ticket>> GetTicketsAsync(Guid id, PaginationRequestDto paginateRequest);
    public Task<PaginationResponseDto<Ticket>> FindTicketsAsync(Guid userId, SearchTicketsRequestDto searchTicketsRequest);

    public Task CreateTicketAsync(Ticket ticket);
    public Task<Ticket?> GetTicketByIdAsync(Guid id);
    public Task UpdateTicketAsync(Ticket ticket);
    public Task DeleteTicketAsync(Ticket ticket);
    public Task<bool> CheckIfTicketExistAsync(Guid id);
    Task CheckTicketUpdateIds(Ticket ticket);
}
