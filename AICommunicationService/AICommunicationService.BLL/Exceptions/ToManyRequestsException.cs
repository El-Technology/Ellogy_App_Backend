namespace AICommunicationService.BLL.Exceptions;
public class ToManyRequestsException : Exception
{
    public new string? Message { get; private set; }
    public ToManyRequestsException(string? message)
    {
        Message = message;
    }
}
