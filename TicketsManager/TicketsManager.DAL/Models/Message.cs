#pragma warning disable CS8618
namespace TicketsManager.DAL.Models;

public class Message
{
    public Guid Id { get; set; }

    public string Sender { get; set; }
    public string Content { get; set; }

    public Ticket Ticket { get; set; }
    public Guid TicketId { get; set; }
}
