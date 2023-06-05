namespace UserManager.BLL.Exceptions;

public class UserAlreadyExistException : Exception
{
    private const string MessageTemplate = "User with email {0} already exists.";
    public new string Message { get; private set; }

    public UserAlreadyExistException(string email)
    {
        Message = string.Format(MessageTemplate, email);
    }
}
