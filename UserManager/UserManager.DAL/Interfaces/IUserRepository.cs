using UserManager.DAL.Models;

namespace UserManager.DAL.Interfaces;

public interface IUserRepository
{
    public Task AddUserAsync(User user);
    public Task UpdateUserAsync(User user);
    public ValueTask<User?> GetUserByIdAsync(Guid id);
    public Task<User?> GetUserByForgetPasswordIdAsync(Guid id);

    public Task<bool> CheckEmailIsExistAsync(string email);
    public Task<User?> GetUserByEmailAsync(string email);
}
