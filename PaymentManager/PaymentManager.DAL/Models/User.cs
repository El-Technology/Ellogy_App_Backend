using PaymentManager.DAL.Enums;

namespace PaymentManager.DAL.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public RoleEnum Role { get; set; }
        public int TotalPurchasedTokens { get; set; }
    }
}
