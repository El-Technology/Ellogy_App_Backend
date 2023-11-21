namespace AICommunicationService.DAL.Models
{
    public class Wallet
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int Balance { get; set; }
    }
}
