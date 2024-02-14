namespace UserManager.Common;

public static class EnvironmentVariables
{
    public static string AzureServiceBusConnectionString
    {
        get
        {
            var variable = Environment.GetEnvironmentVariable("AZURE_SERVICE_BUS_CONNECTION_STRING");
            return variable ??= "default_AZURE_SERVICE_BUS_CONNECTION_STRING";
        }
    }

    public static string ConnectionString
    {
        get
        {
            var variable = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            return variable ??= "default_CONNECTION_STRING";
        }
    }

    public static string ConnectionStringPayment
    {
        get
        {
            var variable = Environment.GetEnvironmentVariable("CONNECTIONSTRING_PAYMENT");
            return variable ??= "default_CONNECTIONSTRING_PAYMENT";
        }
    }

    public static string JwtSecretKey
    {
        get
        {
            var variable = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            return variable ??= "default_JWT_SECRET_KEY_HAVE_32_S";
        }
    }

    public static string EmailClientConnectionString
    {
        get
        {
            var variable = Environment.GetEnvironmentVariable("EMAIL_CLIENT_CONNECTION_STRING");
            return variable ??= "default_EMAIL_CLIENT_CONNECTION_STRING";
        }
    }

    public static string BlobStorageConnectionString
    {
        get
        {
            var variable = Environment.GetEnvironmentVariable("BLOB_STORAGE_CONNECTION_STRING");
            return variable ??= "default_BLOB_STORAGE_CONNECTION_STRING";
        }
    }
}