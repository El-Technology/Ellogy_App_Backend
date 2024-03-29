using TicketsManager.DAL.Enums;

namespace TicketsManager.DAL.Models;

public class ActionHistory
{
    public Guid Id { get; set; }
    public ActionHistoryEnum ActionHistoryEnum { get; set; }
    public TicketCurrentStepEnum TicketCurrentStepEnum { get; set; }
    public string? UserEmail { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime ActionTime { get; set; }

    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; }
}
