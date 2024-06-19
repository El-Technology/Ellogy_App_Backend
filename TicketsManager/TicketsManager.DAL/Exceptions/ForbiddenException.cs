namespace TicketsManager.DAL.Exceptions;
public class ForbiddenException : Exception
{
    private const string MessageTemplate = "User with id {0} has no permission to access this resource";
    public new string Message { get; set; }

    public new Exception? InnerException { get; set; }

    public ForbiddenException(Guid userId, Exception? innerException = null)
    {
        Message = string.Format(MessageTemplate, userId);
        InnerException = innerException;
    }
}