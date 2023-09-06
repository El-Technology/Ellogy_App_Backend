namespace AICommunicationService.Common.Models.AIRequest
{
    public class CreateConversationRequest
    {
        public string SystemMessage { get; set; }
        public string UserInput { get; set; }
        public int Temperature { get; set; }
    }
}
