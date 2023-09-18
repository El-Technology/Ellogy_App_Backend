namespace AICommunicationService.DAL.Models
{
    public class AIPrompt
    {
        public string TemplateName { get; set; }
        public string? Input { get; set; }
        public string Value { get; set; }
    }
}
