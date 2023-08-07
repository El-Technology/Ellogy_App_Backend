using Microsoft.EntityFrameworkCore;
using TicketsManager.DAL.Context;
using TicketsManager.DAL.Exceptions;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TicketsManagerDbContext _context;

    public UserRepository(TicketsManagerDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserAsync(Guid id)
    {
        var user = await _context.Users
                       .AsNoTracking()
                       .Include(u => u.UserTickets)
                       .ThenInclude(t => t.TicketMessages)
                       .Include(u => u.UserTickets)
                       .ThenInclude(t => t.TicketSummaries)
                       .Include(u => u.UserTickets)
                       .ThenInclude(d => d.Usecases)
                       .Include(u => u.UserTickets)
                       .ThenInclude(u => u.Notifications)
                       .FirstOrDefaultAsync(e => e.Id == id)
                   ?? throw new EntityNotFoundException(typeof(User));

        foreach (var ticket in user.UserTickets)
            ticket.TicketMessages = ticket.TicketMessages.OrderBy(e => e.SendTime).ToList();

        return user;
    }

    public Task<bool> CheckIfUserExistAsync(Guid id)
    {
        return _context.Users.AnyAsync(e => e.Id == id);
    }
}
