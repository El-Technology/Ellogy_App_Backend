using AICommunicationService.BLL.Dtos;
using AICommunicationService.Common.Enums;
using AICommunicationService.Common.Models.AIRequest;

namespace AICommunicationService.BLL.Interfaces;
public interface IGroqAiRequestService
{
    Task<CommunicationResponseModel> PostAiRequestAsync(MessageRequest request, AiRequestType aiRequestType, AiModelEnum aiModelEnum);
}
