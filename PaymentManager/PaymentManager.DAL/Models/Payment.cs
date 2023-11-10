namespace PaymentManager.DAL.Models
{
    public class Payment
    {
        public Guid Id { get; set; }
        public string SessionId { get; set; }
        public string? PaymentId { get; set; }
        public string UserEmail { get; set; }
        public Guid ProductId { get; set; }
        public string Status { get; set; }
        public bool UpdatedBallance { get; set; }
        public Guid UserId { get; set; }
        //public DateTime? CreateRequestDate { get; set; }
        //public DateTime? CompleteRequestDate { get; set; }
    }
}
