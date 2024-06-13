using AICommunicationService.Common.Enums;
using AICommunicationService.Common.Models.AIRequest;

namespace AICommunicationService.BLL.Dtos;
public class ConversationRequestDto
{
    public CreateConversationRequest CreateConversationRequest { get; set; } = new CreateConversationRequest();
    public AccountPlan AccountPlan { get; set; }
}
