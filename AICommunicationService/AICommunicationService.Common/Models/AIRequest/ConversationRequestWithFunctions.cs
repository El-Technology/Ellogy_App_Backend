namespace AICommunicationService.Common.Models.AIRequest
{
    public class ConversationRequestWithFunctions
    {
        public float Temperature { get; set; }
        public string UserInput { get; set; }
        public string Template { get; set; }
        public string? Functions { get; set; }
        public string Url { get; set; }
    }
}
