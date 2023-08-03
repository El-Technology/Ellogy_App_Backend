namespace TicketsManager.DAL.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Sms { get; set; }
        public bool Email { get; set; }
        public bool Push { get; set; }

        public Ticket Ticket { get; set; }
        public Guid TicketId { get; set; }
    }
}
