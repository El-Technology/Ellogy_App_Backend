namespace PaymentManager.BLL.Models
{
    public class CreateSessionRequest : SignalRModel
    {
        public string SuccessUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}
