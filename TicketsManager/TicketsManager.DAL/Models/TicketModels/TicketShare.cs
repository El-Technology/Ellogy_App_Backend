using TicketsManager.DAL.Enums;

namespace TicketsManager.DAL.Models.TicketModels;
public class TicketShare
{
    public Guid Id { get; set; }
    public Guid SharedUserId { get; set; }
    public DateTime GivenAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public SharePermissionEnum Permission { get; set; }
    public TicketCurrentStepEnum? TicketCurrentStep { get; set; }
    public SubStageEnum? SubStageEnum { get; set; }

    public Ticket Ticket { get; set; } = null!;
    public Guid TicketId { get; set; }
}
