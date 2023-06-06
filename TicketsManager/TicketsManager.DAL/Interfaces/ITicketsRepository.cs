using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Interfaces;

public interface ITicketsRepository
{
    Task<ICollection<Ticket>> GetAllTicketsAsync(Guid id);
    Task CreateTicketAsync(Ticket ticket);
}
