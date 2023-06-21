using Microsoft.EntityFrameworkCore;
using UserManager.DAL.Context;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.DAL.Repositories;

public class ForgotPasswordRepository : IForgotPasswordRepository
{
    private readonly UserManagerDbContext _context;

    public ForgotPasswordRepository(UserManagerDbContext context)
    {
        _context = context;
    }

    public async Task AddForgotTokenAsync(ForgotPassword forgotPasswordEntry)
    {
        await _context.ForgotPasswords.AddAsync(forgotPasswordEntry);
        await _context.SaveChangesAsync();
    }

    public Task<bool> ValidateResetRequestAsync(Guid userId, string token)
    {
        return _context.ForgotPasswords.AnyAsync(e => e.Id == userId
                                                        && e.Token == token
                                                        && e.ExpireDate <= DateTime.UtcNow
                                                        && e.IsValid);
    }
}
