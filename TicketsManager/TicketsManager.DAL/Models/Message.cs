using TicketsManager.DAL.Enums;

namespace TicketsManager.DAL.Models;

public class Message
{
    public Guid Id { get; set; }

    public string Sender { get; set; }
    public string Content { get; set; }
    public DateTime SendTime { get; set; }
    public MessageActionTypeEnum? ActionType { get; set; }
    public MessageActionStateEnum? ActionState { get; set; }
    public MessageStageEnum? Stage { get; set; }
    public SubStageEnum? SubStage { get; set; }

    public Ticket Ticket { get; set; }
    public Guid TicketId { get; set; }
}
