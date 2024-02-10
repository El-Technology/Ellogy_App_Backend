using PaymentManager.DAL.Enums;

namespace PaymentManager.BLL.Helpers;

/// <summary>
///     This class contains helper methods for subscription related operations
/// </summary>
public static class SubscriptionHelper
{
    /// <summary>
    ///     This method returns the amount of tokens for a given account plan
    /// </summary>
    /// <param name="accountPlan"></param>
    /// <returns></returns>
    public static int GetAmountOfTokens(AccountPlan accountPlan)
    {
        return accountPlan switch
        {
            AccountPlan.Basic => 45000,
            AccountPlan.Free => 15000,
            _ => 0
        };
    }
}