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
            AccountPlan.Starter => 0, //unlimited access
            AccountPlan.Basic => 0, //20k will be given, when wallet will be created firstly
            _ => 0
        };
    }
}