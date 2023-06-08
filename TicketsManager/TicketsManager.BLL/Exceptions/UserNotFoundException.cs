namespace TicketsManager.BLL.Exceptions;

public class UserNotFoundException : Exception
{
    private const string MessageTemplate = "User with id {0} not found";
    public new string Message { get; set; }

    public new Exception? InnerException { get; set; }

    public UserNotFoundException(Guid id, Exception? innerException = null)
    {
        Message = string.Format(MessageTemplate, id);
        InnerException = innerException;
    }
}
