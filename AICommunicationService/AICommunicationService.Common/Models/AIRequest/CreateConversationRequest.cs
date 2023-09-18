namespace AICommunicationService.Common.Models.AIRequest
{
    public class CreateConversationRequest
    {
        public string TemplateName { get; set; }
        public string UserInput { get; set; }
        public int Temperature { get; set; }
    }
}
