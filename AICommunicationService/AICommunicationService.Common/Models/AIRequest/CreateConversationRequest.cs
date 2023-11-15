using AICommunicationService.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace AICommunicationService.Common.Models.AIRequest
{
    public class CreateConversationRequest
    {
        [Required]
        public AiModelEnum AiModelEnum { get; set; }
        public string TemplateName { get; set; }
        public string UserInput { get; set; }
        public float Temperature { get; set; }
        public Guid TicketId { get; set; }
    }
}
