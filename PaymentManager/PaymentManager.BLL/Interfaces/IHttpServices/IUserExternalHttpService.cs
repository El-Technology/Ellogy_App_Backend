using PaymentManager.BLL.Models;
using PaymentManager.DAL.Enums;

namespace PaymentManager.BLL.Interfaces.IHttpServices;
public interface IUserExternalHttpService
{
    Task AddStripeCustomerIdAsync(Guid userId, string stripeId);
    Task<UserDto> GetUserByIdAsync(Guid userId);
    Task RemoveStripeCustomerIdAsync(Guid userId);
    Task UpdateAccountPlanAsync(Guid userId, AccountPlan? accountPlan);
    Task UpdateTotalPurchasedTokensAsync(Guid userId, int totalPurchasedTokens);
}
