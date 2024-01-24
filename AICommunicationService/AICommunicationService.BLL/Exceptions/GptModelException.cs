namespace AICommunicationService.BLL.Exceptions
{
    public class GptModelException : Exception
    {
        public new string Message { get; private set; }
        public GptModelException(string message)
        {
            Message = message;
        }
    }
}
