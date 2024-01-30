namespace PaymentManager.DAL.Models
{
    public class Payment
    {
        public Guid Id { get; set; }
        public string? SessionId { get; set; }
        public string? PaymentId { get; set; }
        public string? InvoiceId { get; set; }
        public string? Mode { get; set; }
        public string? ProductName { get; set; }
        public string? UserEmail { get; set; }
        public int AmountOfPoints { get; set; }
        public string? Status { get; set; }
        public bool UpdatedBallance { get; set; }
        public Guid UserId { get; set; }
    }
}
