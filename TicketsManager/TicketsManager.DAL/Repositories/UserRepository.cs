using TicketsManager.DAL.Context;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TicketsManagerDbContext _dbContext;

    public UserRepository(TicketsManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ValueTask<User?> GetUserAsync(Guid id)
    {
        return _dbContext.Users.FindAsync(id);
    }
}
