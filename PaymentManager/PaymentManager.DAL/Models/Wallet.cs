namespace PaymentManager.DAL.Models
{
    public class Wallet
    {
        public Guid Id { get; set; }
        public string UserEmail { get; set; }
        public int Balance { get; set; }
    }
}
