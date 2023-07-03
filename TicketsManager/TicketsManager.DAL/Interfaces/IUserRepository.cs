using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Interfaces;

public interface IUserRepository
{
    public Task<User?> GetUserAsync(Guid id);
    public Task<bool> CheckIfUserExistAsync(Guid id);
}
