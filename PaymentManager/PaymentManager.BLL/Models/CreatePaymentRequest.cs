namespace PaymentManager.BLL.Models
{
    public class CreatePaymentRequest : CreateSessionRequest
    {
        public int AmountOfPoints { get; set; }
    }
}
