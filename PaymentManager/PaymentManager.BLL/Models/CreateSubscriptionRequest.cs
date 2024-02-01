namespace PaymentManager.BLL.Models
{
    public class CreateSubscriptionRequest : CreateSessionRequest
    {
        public string ProductId { get; set; }
    }
}
