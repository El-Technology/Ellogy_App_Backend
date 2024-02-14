using UserManager.DAL.Models;

namespace UserManager.DAL.Interfaces;

public interface IForgotPasswordRepository
{
    /// <summary>
    ///     This method adds a new forgot password token to the database.
    /// </summary>
    /// <param name="forgotPasswordEntry"></param>
    /// <returns></returns>
    public Task AddForgotTokenAsync(ForgotPassword forgotPasswordEntry);

    /// <summary>
    ///     This method validates the reset request.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<bool> ValidateResetRequestAsync(Guid id, string token);

    /// <summary>
    ///     This method invalidates the token.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task InvalidateTokenAsync(Guid id);
}