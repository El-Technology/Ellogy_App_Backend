using TicketsManager.DAL.Enums;

#pragma warning disable CS8618
namespace TicketsManager.DAL.Models;

public class Ticket
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Summary { get; set; }
    public string Comment { get; set; }
    public DateOnly CreatedDate { get; set; }
    public DateOnly UpdatedDate { get; set; }
    public TicketStatusEnum Status { get; set; }
    public ICollection<Message> TicketMessages { get; set; } = new List<Message>();

    public Guid UserId { get; set; }
    public User User { get; set; }
}