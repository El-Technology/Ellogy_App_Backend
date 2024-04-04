using PaymentManager.DAL.Enums;

namespace PaymentManager.BLL.Models;

public class UserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string StripeCustomerId { get; set; } = string.Empty;
    public RoleEnum Role { get; set; }
    public int TotalPurchasedPoints { get; set; }
    public AccountPlan? AccountPlan { get; set; }
}