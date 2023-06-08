namespace TicketsManager.BLL.Exceptions;

public class TicketNotFoundException : Exception
{
    private const string MessageTemplate = "Ticket with id: {0} not found";
    public new string Message { get; private set; }

    public TicketNotFoundException(Guid id)
    {
        Message = string.Format(MessageTemplate, id);
    }
}
