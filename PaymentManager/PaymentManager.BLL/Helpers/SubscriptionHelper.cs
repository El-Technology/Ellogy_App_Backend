using PaymentManager.DAL.Enums;
using Stripe;

namespace PaymentManager.BLL.Helpers
{
    public static class SubscriptionHelper
    {
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
}
