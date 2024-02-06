namespace PaymentManager.BLL.Helpers
{
    public static class SubscriptionHelper
    {
        public static int? GetSubscriptionCode(string subscriptionName)
        {
            return subscriptionName switch
            {
                "Basic" => 1,
                "Free" => 0,
                _ => null
            };
        }
    }
}
