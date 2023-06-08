#pragma warning disable CS8618
namespace TicketsManager.DAL.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public ICollection<Ticket> UserTickets { get; set; } = new List<Ticket>();
    }
}