using Microsoft.EntityFrameworkCore;
using TicketsManager.DAL.Context;
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

    public Task<User?> GetUserAsync(Guid id)
    {
        return _context.Users
            .AsNoTracking()
            .Include(e => e.UserTickets)
            .ThenInclude(e => e.TicketMessages)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public Task<bool> CheckIfUserExistAsync(Guid id)
    {
        return _context.Users.AnyAsync(e => e.Id == id);
    }
}
