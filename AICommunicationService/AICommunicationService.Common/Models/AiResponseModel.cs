using Newtonsoft.Json;

namespace AICommunicationService.Common.Models
{
    public class Choice
    {
        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("finish_reason")]
        public string? FinishReason { get; set; }

        [JsonProperty("message")]
        public Message? Message { get; set; }

        [JsonProperty("content_filter_results")]
        public ContentFilterResults? ContentFilterResults { get; set; }

        [JsonProperty("delta")]
        public Delta? Delta { get; set; }
    }

    public class Delta
    {
        [JsonProperty("content")]
        public string? Content { get; set; }
    }

    public class ContentFilterResults
    {
        [JsonProperty("hate")]
        public Hate Hate { get; set; }

        [JsonProperty("self_harm")]
        public SelfHarm SelfHarm { get; set; }

        [JsonProperty("sexual")]
        public Sexual Sexual { get; set; }

        [JsonProperty("violence")]
        public Violence Violence { get; set; }
    }

    public class FunctionCall
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("arguments")]
        public string Arguments { get; set; }
    }

    public class Hate
    {
        [JsonProperty("filtered")]
        public bool Filtered { get; set; }

        [JsonProperty("severity")]
        public string Severity { get; set; }
    }

    public class Message
    {
        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("content")]
        public string? Content { get; set; }

        [JsonProperty("function_call")]
        public FunctionCall? FunctionCall { get; set; }
    }

    public class PromptFilterResult
    {
        [JsonProperty("prompt_index")]
        public int PromptIndex { get; set; }

        [JsonProperty("content_filter_results")]
        public ContentFilterResults ContentFilterResults { get; set; }
    }

    public class AiResponseModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("created")]
        public int Created { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("prompt_filter_results")]
        public List<PromptFilterResult> PromptFilterResults { get; set; }

        [JsonProperty("choices")]
        public List<Choice>? Choices { get; set; }

        [JsonProperty("usage")]
        public Usage Usage { get; set; }
    }

    public class SelfHarm
    {
        [JsonProperty("filtered")]
        public bool Filtered { get; set; }

        [JsonProperty("severity")]
        public string Severity { get; set; }
    }

    public class Sexual
    {
        [JsonProperty("filtered")]
        public bool Filtered { get; set; }

        [JsonProperty("severity")]
        public string Severity { get; set; }
    }

    public class Usage
    {
        [JsonProperty("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonProperty("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonProperty("total_tokens")]
        public int TotalTokens { get; set; }
    }

    public class Violence
    {
        [JsonProperty("filtered")]
        public bool Filtered { get; set; }

        [JsonProperty("severity")]
        public string Severity { get; set; }
    }

    public class FunctionDefinition
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("parameters")]
        public object Parameters { get; set; }
    }
}
