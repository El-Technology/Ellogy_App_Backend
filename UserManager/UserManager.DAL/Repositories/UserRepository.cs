using Microsoft.EntityFrameworkCore;
using UserManager.DAL.Context.UserContext;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManagerDbContext _context;

    public UserRepository(UserManagerDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc cref="IUserRepository.AddUserAsync" />
    public async Task AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="IUserRepository.DeleteUserAsync" />
    public async Task DeleteUserAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="IUserRepository.CheckEmailIsExistAsync" />
    public Task<bool> CheckEmailIsExistAsync(string email)
    {
        return _context.Users.AnyAsync(e => string.Equals(email.ToLower(), e.Email.ToLower()));
    }

    /// <inheritdoc cref="IUserRepository.GetUserByIdAsync" />
    public ValueTask<User?> GetUserByIdAsync(Guid id)
    {
        return _context.Users.FindAsync(id);
    }

    /// <inheritdoc cref="IUserRepository.GetUserByForgetPasswordIdAsync" />
    public async Task<User?> GetUserByForgetPasswordIdAsync(Guid id)
    {
        var userId = (await _context.ForgotPasswords.FindAsync(id))?.UserId;
        return await _context.Users.FindAsync(userId);
    }

    /// <inheritdoc cref="IUserRepository.UpdateUserAsync" />
    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="IUserRepository.GetUserByEmailAsync" />
    public Task<User?> GetUserByEmailAsync(string email)
    {
        return _context.Users.FirstOrDefaultAsync(e => string.Equals(email.ToLower(), e.Email.ToLower()));
    }
}