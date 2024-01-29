namespace PaymentManager.BLL.Models
{
    public class CreateSubscriptionRequest : CreateSessionRequest
    {
        public string PriceId { get; set; }
    }
}
