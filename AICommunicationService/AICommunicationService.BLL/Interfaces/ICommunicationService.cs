namespace AICommunicationService.BLL.Interfaces
{
    public interface ICommunicationService
    {
        Task<string> SendMessageAsync(string message);
    }
}
