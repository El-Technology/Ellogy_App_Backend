namespace PaymentManager.BLL.Models
{
    public class CreatePaymentRequest
    {
        public string ConnectionId { get; set; }
        public string SignalMethodName { get; set; }
        public int AmountOfPoints { get; set; }
        public string SuccessUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}
