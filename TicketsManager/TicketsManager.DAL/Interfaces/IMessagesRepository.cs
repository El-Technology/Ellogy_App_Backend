using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Interfaces;

public interface IMessagesRepository
{
    Task CreateMessageAsync(Message message);
}
