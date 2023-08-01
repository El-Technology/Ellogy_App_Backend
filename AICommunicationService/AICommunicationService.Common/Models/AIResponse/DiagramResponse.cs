using Newtonsoft.Json;

namespace AICommunicationService.Common.Models.AIResponse
{
    public class DiagramResponse
    {
        [JsonProperty("diagrams")]
        public List<DiagramItem> Diagrams { get; set; }
    }

    public class DiagramItem
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
