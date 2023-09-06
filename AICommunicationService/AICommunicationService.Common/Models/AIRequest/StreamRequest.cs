namespace AICommunicationService.Common.Models.AIRequest
{
    public class StreamRequest : CreateConversationRequest
    {
        public string ConnectionId { get; set; }
        public string SignalMethodName { get; set; }
    }
}
