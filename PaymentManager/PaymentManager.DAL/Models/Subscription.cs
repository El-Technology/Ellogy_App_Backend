namespace PaymentManager.DAL.Models
{
    public class Subscription
    {
        public Guid Id { get; set; }
        public string SubscriptionStripeId { get; set; }
        public Guid UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsCanceled { get; set; } = false;
    }
}
