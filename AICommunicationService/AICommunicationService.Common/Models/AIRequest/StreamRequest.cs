namespace AICommunicationService.Common.Models.AIRequest
{
    public class StreamRequest
    {
        public string SystemMessage { get; set; }
        public string UserInput { get; set; }
        public int Temperature { get; set; }
        public string ConnectionId { get; set; }
        public string SignalMethodName { get; set; }
    }
}
