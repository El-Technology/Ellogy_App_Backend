using UserManager.Common.Helpers;
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

    public async Task<bool> ValidateResetRequestAsync(Guid id, string token)
    {
        var forgotPasswordEntry = await _context.ForgotPasswords.FindAsync(id);

        return forgotPasswordEntry is not null &&
               CryptoHelper.GetHash(forgotPasswordEntry.Token) == token &&
               forgotPasswordEntry.ExpireDate >= DateTime.UtcNow &&
               forgotPasswordEntry.IsValid;
    }

    public async Task InvalidateTokenAsync(Guid id)
    {
        var entry = await _context.ForgotPasswords.FindAsync(id);
        if (entry is null)
            return;

        entry.IsValid = false;
        await _context.SaveChangesAsync();
    }
}
