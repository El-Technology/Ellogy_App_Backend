using AICommunicationService.Common.Models.AIRequest;

namespace AICommunicationService.BLL.Interfaces
{
    public interface IAzureOpenAiRequestService
    {
        Task PostAiRequestAsStreamAsync(ConversationRequestWithFunctions request, Func<string, Task> onDataReceived);
        Task<string?> PostAiRequestAsync(ConversationRequestWithFunctions request);
        Task<string?> PostAiRequestWithFunctionAsync(ConversationRequestWithFunctions request);
    }
}
