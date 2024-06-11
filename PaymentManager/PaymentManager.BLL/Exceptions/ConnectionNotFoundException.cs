namespace PaymentManager.BLL.Exceptions;
public class ConnectionNotFoundException : Exception
{
    public ConnectionNotFoundException(string message) : base(message)
    {
    }
}
