namespace TicketsManager.DAL.Models
{
    public class TicketTableValue
    {
        public Guid Id { get; set; }
        public string Value { get; set; }

        public TicketTable TicketTable { get; set; }
        public Guid TicketTableId { get; set; }
    }
}
