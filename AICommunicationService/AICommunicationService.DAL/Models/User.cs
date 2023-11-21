namespace AICommunicationService.DAL.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public int TotalPointsUsage { get; set; }
        public int TotalPurchasedPoints { get; set; }
    }
}
