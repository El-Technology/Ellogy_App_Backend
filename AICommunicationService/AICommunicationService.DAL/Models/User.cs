namespace AICommunicationService.DAL.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public int TotalTokensUsage { get; set; }
        public int TotalPurchasedTokens { get; set; }
    }
}
