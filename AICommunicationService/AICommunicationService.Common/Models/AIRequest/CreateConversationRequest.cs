using AICommunicationService.Common.Enums;

namespace AICommunicationService.Common.Models.AIRequest
{
    public class CreateConversationRequest
    {
        public AiModelEnum AiModelEnum { get; set; }
        public string TemplateName { get; set; }
        public string UserInput { get; set; }
        public float Temperature { get; set; }
    }
}
