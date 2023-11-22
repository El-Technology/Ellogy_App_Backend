namespace AICommunicationService.BLL.Exceptions
{
    public class BalanceException : Exception
    {
        public new string Message { get; private set; }
        public BalanceException(string message)
        {
            Message = message;
        }
    }
}
