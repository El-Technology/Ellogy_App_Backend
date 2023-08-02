namespace TicketsManager.DAL.Models
{
    public class TicketTable
    {
        public Guid Id { get; set; }
        public string TableKey { get; set; }
        public ICollection<TicketTableValue> TableValues { get; set; } = new List<TicketTableValue>();

        public Ticket Ticket { get; set; }
        public Guid TicketId { get; set; }
    }
}
