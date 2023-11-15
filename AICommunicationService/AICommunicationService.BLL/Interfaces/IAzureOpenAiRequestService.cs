using AICommunicationService.BLL.Dtos;
using AICommunicationService.Common.Models.AIRequest;

namespace AICommunicationService.BLL.Interfaces
{
    public interface IAzureOpenAiRequestService
    {
        Task PostAiRequestAsStreamAsync(MessageRequest request, Func<string, Task> onDataReceived);
        Task<CommunicationResponseModel> PostAiRequestAsync(MessageRequest request);
        Task<CommunicationResponseModel> PostAiRequestWithFunctionAsync(MessageRequest request);
    }
}
