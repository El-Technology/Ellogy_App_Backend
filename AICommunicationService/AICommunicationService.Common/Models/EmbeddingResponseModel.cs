namespace AICommunicationService.Common.Models
{
    public class Datum
    {
        public string @object { get; set; }
        public int index { get; set; }
        public List<double> embedding { get; set; }
    }

    public class EmbeddingResponseModel
    {
        public string @object { get; set; }
        public List<Datum> data { get; set; }
        public string model { get; set; }
        public PromptUsage usage { get; set; }
    }

    public class PromptUsage
    {
        public int prompt_tokens { get; set; }
        public int total_tokens { get; set; }
    }
}
