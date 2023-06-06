using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Interfaces;

public interface ITicketsRepository
{
    Task<ICollection<Ticket>> GetAllTicketsAsync(Guid id);
    Task CreateTicketAsync(Ticket ticket);
    Task<Ticket?> GetTicketByIdAsync(Guid id);
    Task UpdateTicketAsync(Ticket ticket);
    Task DeleteTicketAsync(Ticket ticket);
    Task<bool> CheckIfTicketExistAsync(Guid id);
}
