namespace AICommunicationService.BLL.Dtos
{
    public class UpdatePrompt
    {
        public string Description { get; set; }
        public string? Functions { get; set; }
        public string Value { get; set; }
        public string Input { get; set; }
    }
}
