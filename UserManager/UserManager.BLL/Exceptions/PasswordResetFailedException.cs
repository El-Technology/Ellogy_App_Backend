namespace UserManager.BLL.Exceptions;

public class PasswordResetFailedException : Exception
{
    public PasswordResetFailedException(string? message = "Password reset is failed") : base(message)
    { }
}
