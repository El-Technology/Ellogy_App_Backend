namespace PaymentManager.BLL.Models
{
    public class StreamRequest
    {
        public string ConnectionId { get; set; }
        public string SignalMethodName { get; set; }
        public Guid ProductId { get; set; }
    }
}
