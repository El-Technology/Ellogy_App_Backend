namespace PaymentManager.Common;

public static class EnvironmentVariables
{
    public static string WebhookKey
    {
        get
        {
            var variable = Environment.GetEnvironmentVariable("WEBHOOK_KEY");
            return variable ?? "WEBHOOK_KEY";
        }
    }

    public static string ConnectionStringPayment
    {
        get
        {
            var variable = Environment.GetEnvironmentVariable("CONNECTIONSTRING_PAYMENT");
            return variable ?? "default_CONNECTION_STRING";
        }
    }

    public static string ConnectionString
    {
        get
        {
            var variable = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            return variable ?? "default_CONNECTION_STRING";
        }
    }

    public static string SecretKey
    {
        get
        {
            var variable = Environment.GetEnvironmentVariable("SECRET_KEY");
            return variable ?? "default_SECRET_KEY";
        }
    }

    public static string AzureServiceBusConnectionString
    {
        get
        {
            var variable = Environment.GetEnvironmentVariable("AZURE_SERVICE_BUS_CONNECTION_STRING_PAYMENT");
            return variable ?? "default_AZURE_SERVICE_BUS_CONNECTION_STRING";
        }
    }

    public static string JwtSecretKey
    {
        get
        {
            var variable = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            return variable ?? "default_JWT_SECRET_KEY_HAVE_32_S";
        }
    }
}