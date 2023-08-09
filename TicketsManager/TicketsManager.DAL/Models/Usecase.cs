namespace TicketsManager.DAL.Models
{
    public class Usecase
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public ICollection<TicketTable> Tables { get; set; }
        public ICollection<TicketDiagram> Diagrams { get; set; }

        public Ticket Ticket { get; set; }
        public Guid TicketId { get; set; }
    }
}
