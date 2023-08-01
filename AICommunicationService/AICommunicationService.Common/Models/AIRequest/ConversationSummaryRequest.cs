namespace AICommunicationService.Common.Models.AIRequest
{
    public class ConversationSummaryRequest
    {
        public string CurrentSummary { get; set; }
        public string Human { get; set; }
        public string AI { get; set; }
    }
}
