using UserManager.DAL.Models;

namespace UserManager.DAL.Interfaces;

public interface IForgotPasswordRepository
{
    public Task AddForgotTokenAsync(ForgotPassword forgotPasswordEntry);
    public Task<bool> ValidateResetRequestAsync(Guid id, string token);
    public Task InvalidateTokenAsync(Guid id);
}
