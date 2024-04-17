using System.Collections.Specialized;

namespace AICommunicationService.Common.Models.AIRequest;

public class MessageRequest
{
    public float Temperature { get; set; }
    public string UserInput { get; set; }
    public OrderedDictionary? ConversationHistory { get; set; }
    public string Template { get; set; }
    public string? Context { get; set; } = null;
    public string? Functions { get; set; }
    public string Url { get; set; }
}
