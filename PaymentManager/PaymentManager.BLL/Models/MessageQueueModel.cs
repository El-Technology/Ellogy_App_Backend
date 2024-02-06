namespace PaymentManager.BLL.Models
{
    public class MessageQueueModel<T> where T : class
    {
        public string Type { get; set; }
        public T CreateOptions { get; set; }
    }
}
