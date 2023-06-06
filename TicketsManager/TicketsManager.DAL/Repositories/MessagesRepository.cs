using TicketsManager.DAL.Context;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Repositories;

public class MessagesRepository : IMessagesRepository
{
    private readonly TicketsManagerDbContext _context;

    public MessagesRepository(TicketsManagerDbContext context)
    {
        _context = context;
    }

    public async Task CreateMessageAsync(Message message)
    {
        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();
    }
}
