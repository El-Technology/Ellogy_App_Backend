using TicketsManager.DAL.Models.UserStoryTests;

namespace TicketsManager.DAL.Models;

public class Usecase
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public int Order { get; set; }

    public ICollection<TicketTable> Tables { get; set; } = new List<TicketTable>();
    public ICollection<TicketDiagram> Diagrams { get; set; } = new List<TicketDiagram>();
    public ICollection<TicketSummary> TicketSummaries { get; set; } = new List<TicketSummary>();

    public Ticket Ticket { get; set; }
    public Guid TicketId { get; set; }

    public UserStoryTest? UserStoryTest { get; set; }
}
