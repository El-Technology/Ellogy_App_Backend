namespace UserManager.BLL.Exceptions;

public class FailedLoginException : Exception
{
    public new string Message { get; set; } = "Login with this credentials was failed";
    public FailedLoginException()
    { }
}
