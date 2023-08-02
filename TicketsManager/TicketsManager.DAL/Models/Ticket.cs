using TicketsManager.DAL.Enums;

namespace TicketsManager.DAL.Models;

public class Ticket
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string? Context { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public TicketStatusEnum Status { get; set; }
    public TicketCurrentStepEnum CurrentStep { get; set; }

    public ICollection<Message> TicketMessages { get; set; } = new List<Message>();
    public ICollection<TicketSummary> TicketSummaries { get; set; } = new List<TicketSummary>();
    public ICollection<TicketTable> TicketTables { get; set; } = new List<TicketTable>();
    public ICollection<TicketDiagram> TicketDiagrams { get; set; } = new List<TicketDiagram>();

    public Guid UserId { get; set; }
    public User User { get; set; }
}