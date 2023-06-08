using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Interfaces;

public interface IUserRepository
{
    ValueTask<User?> GetUserAsync(Guid id);
}
