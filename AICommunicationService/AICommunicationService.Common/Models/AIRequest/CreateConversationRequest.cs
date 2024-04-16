using AICommunicationService.Common.Constants;
using AICommunicationService.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace AICommunicationService.Common.Models.AIRequest;

public class CreateConversationRequest
{
    [Required]
    public AiModelEnum AiModelEnum { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public string UserInput { get; set; } = string.Empty;
    public float Temperature { get; set; }
    public bool UseRAG { get; set; }
    public string RagTemplate { get; set; } = RagConstants.RAG_CONTEXT;
    public string? FileName { get; set; } = null;
}
