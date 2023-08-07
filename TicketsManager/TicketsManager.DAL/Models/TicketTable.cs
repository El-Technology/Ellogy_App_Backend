namespace TicketsManager.DAL.Models
{
    public class TicketTable
    {
        public Guid Id { get; set; }
        public string Table { get; set; }

        public Usecase Usecase { get; set; }
        public Guid UsecaseId { get; set; }
    }
}
