using TicketsManager.Common.Helpers.Pagination;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Interfaces;

public interface ITicketsRepository
{
    Task<PaginationResponseDto<Ticket>> GetTicketsAsync(Guid id, PaginationRequestDto paginateRequest);
    Task CreateTicketAsync(Ticket ticket);
    Task<Ticket?> GetTicketByIdAsync(Guid id);
    Task UpdateTicketAsync(Ticket ticket);
    Task DeleteTicketAsync(Ticket ticket);
    Task<bool> CheckIfTicketExistAsync(Guid id);
}
