namespace TicketsManager.BLL.Exceptions;
public class MessageNotFoundException : Exception
{
    private const string MessageTemplate = "Message(s) not found";
    public new string Message { get; private set; }

    public MessageNotFoundException()
    {
        Message = MessageTemplate;
    }
}