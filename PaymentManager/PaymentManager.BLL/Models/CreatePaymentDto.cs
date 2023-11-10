namespace PaymentManager.BLL.Models
{
    public class CreatePaymentDto
    {
        public Guid ProductId { get; set; }
        public string UserEmail { get; set; }
    }
}
