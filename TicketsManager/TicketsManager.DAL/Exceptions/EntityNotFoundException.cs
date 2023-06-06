namespace TicketsManager.DAL.Exceptions;

public class EntityNotFoundException : Exception
{
    private const string MessageTemplate = "Entity: {0} not found";

    public new string Message { get; set; }

    public EntityNotFoundException(string message)
    {
        Message = message;
    }

    public EntityNotFoundException(Type type)
    {
        Message = string.Format(MessageTemplate, type);
    }
}
