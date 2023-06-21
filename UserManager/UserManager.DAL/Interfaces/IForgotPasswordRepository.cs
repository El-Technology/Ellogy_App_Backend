using UserManager.DAL.Models;

namespace UserManager.DAL.Interfaces;

public interface IForgotPasswordRepository
{
    public Task AddForgotTokenAsync(ForgotPassword forgotPasswordEntry);
    public Task<bool> ValidateResetRequestAsync(Guid userId, string token);
}
