namespace TicketsManager.DAL.Models
{
    public class TicketSummary
    {
        public Guid Id { get; set; }
        public string Data { get; set; }
        public bool IsPotential { get; set; }

        public Guid TicketId { get; set; }
        public Ticket Ticket { get; set; }
    }
}
