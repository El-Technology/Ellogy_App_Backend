namespace PaymentManager.Common.Constants
{
    public static class Constants
    {
        public static int OneDollarInPoints = 100;
        public static int NewWalletBalance = 0;
        public static decimal PriceInCents = 100;
        public static decimal OneTokenPrice = 0.01m;

        public static string ApplicationCurrency = "usd";

        public const string PAYMENT_MODE = "payment";
        public const string SUBSCRIPTION_MODE = "subscription";
        public const string SETUP_MODE = "setup";

        public static string PaymentQueueName = "paymentqueue";

        public static string ApplicationJson = "application/json";

        public static string OnConnectedMethod = "OnConnected";
    }
}
