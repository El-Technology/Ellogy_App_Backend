namespace PaymentManager.Common.Constants;

public static class Constants
{
    public const string PaymentMode = "payment";
    public const string SubscriptionMode = "subscription";
    public const string SetupMode = "setup";
    public const string ExpiredStatus = "expired";

    public static int NewWalletBalance = 0;
    public static decimal PriceInCents = 100;
    public static decimal OneTokenPrice = 0.01m;

    public static string ApplicationCurrency = "usd";

    public static string PaymentQueueName = "paymentqueue";

    public static string ApplicationJson = "application/json";

    public static string OnConnectedMethod = "OnConnected";
}