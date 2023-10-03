using AICommunicationService.Common.Models.AIRequest;

namespace AICommunicationService.BLL.Interfaces
{
    public interface IAzureOpenAiRequestService
    {
        Task PostAiRequestAsStreamAsync(MessageRequest request, Func<string, Task> onDataReceived);
        Task<string?> PostAiRequestAsync(MessageRequest request);
        Task<string?> PostAiRequestWithFunctionAsync(MessageRequest request);
    }
}
