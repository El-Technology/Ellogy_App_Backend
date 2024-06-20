using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.DAL.Interfaces;

public interface IMessagesRepository
{
    /// <summary>
    /// Retrieves all messages for a ticket
    /// </summary>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    public Task<List<Message>> GetMessagesByTicketId(Guid ticketId);
}
