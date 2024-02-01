namespace PaymentManager.BLL.Models
{
    public class CreateSessionRequest
    {
        public string ConnectionId { get; set; }
        public string SignalMethodName { get; set; }
        public string SuccessUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}
