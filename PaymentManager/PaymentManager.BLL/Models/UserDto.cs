using PaymentManager.DAL.Enums;

namespace PaymentManager.BLL.Models;

public class UserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string? StripeCustomerId { get; set; }
    public RoleEnum Role { get; set; }
    public int TotalPurchasedPoints { get; set; }
    public AccountPlan? AccountPlan { get; set; }
}