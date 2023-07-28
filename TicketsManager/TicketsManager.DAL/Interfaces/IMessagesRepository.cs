using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Interfaces;

public interface IMessagesRepository
{
    public Task<List<Message>> GetMessagesByTicketId(Guid ticketId);
}
