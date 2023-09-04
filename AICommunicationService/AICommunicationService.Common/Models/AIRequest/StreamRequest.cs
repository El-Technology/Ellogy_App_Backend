namespace AICommunicationService.Common.Models.AIRequest
{
    public class StreamRequest
    {
        public string SystemMessage { get; set; }
        public string UserInput { get; set; }
        public int Temperature { get; set; }
    }
}
