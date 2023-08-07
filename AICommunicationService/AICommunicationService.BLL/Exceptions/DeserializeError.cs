namespace AICommunicationService.BLL.Exceptions
{
    public class DeserializeError : Exception
    {
        public new string Message { get; private set; }
        public DeserializeError(string message)
        {
            Message = message;
        }
    }
}
