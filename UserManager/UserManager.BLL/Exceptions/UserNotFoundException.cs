namespace UserManager.BLL.Exceptions;

public class UserNotFoundException : Exception
{
    private const string MessageTemplate = "User with email {0} not found";
    public new string Message { get; set; }

    public UserNotFoundException(string email)
    {
        Message = string.Format(MessageTemplate, email);
    }
}
