namespace UserManager.BLL.Exceptions;

public class PasswordResetFailedException : Exception
{
    public PasswordResetFailedException() : base("Password reset is failed")
    { }
}
