using UserManager.DAL.Enums;

#pragma warning disable CS8618
namespace UserManager.DAL.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Password { get; set; }
        public string? Organization { get; set; }
        public string? Department { get; set; }
        public string? AvatarLink { get; set; }
        public string Salt { get; set; }
        public RoleEnum Role { get; set; }
        public int TotalPointsUsage { get; set; }
        public int TotalPurchasedPoints { get; set; }
        public bool IsAccountActivated { get; set; } = true;
        public string? VerifyToken { get; set; }
        public string? StripeCustomerId { get; set; }
        public AccountPlan? AccountPlan { get; set; } = null;
        public RefreshToken RefreshToken { get; set; }
    }
}