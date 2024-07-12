namespace TicketsManager.BLL.Exceptions;
public class NotificationNotFoundException : Exception
{
    private const string MessageTemplate = "Notification(s) not found";
    public new string Message { get; private set; }

    public NotificationNotFoundException()
    {
        Message = MessageTemplate;
    }
}
