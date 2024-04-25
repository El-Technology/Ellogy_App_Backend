namespace AICommunicationService.DAL.Models;

public class AIPrompt
{
    public string TemplateName { get; set; }
    public string? Functions { get; set; }
    public string? JsonSample { get; set; }
    public string? Description { get; set; }
    public string? Input { get; set; }
    public string Value { get; set; }
}
