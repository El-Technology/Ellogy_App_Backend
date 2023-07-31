using UserManager.Common.Exceptions;

namespace UserManager.Common;

public static class EnvironmentVariables
{
    public static string AzureServiceBusConnectionString
    {
        get
        {
            var variable = Environment.GetEnvironmentVariable("AZURE_SERVICE_BUS_CONNECTION_STRING");
            return variable is null ? throw new EnvironmentVariableNotFoundException("AZURE_SERVICE_BUS_CONNECTION_STRING") : variable;
        }

    }

    public static string ConnectionString
    {
        get
        {
            var variable = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            return variable is null ? throw new EnvironmentVariableNotFoundException("CONNECTION_STRING") : variable;
        }

    }
    public static string JwtSecretKey
    {
        get
        {
            var variable = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            return variable is null ? throw new EnvironmentVariableNotFoundException("JWT_SECRET_KEY") : variable;
        }

    }
    public static string EmailClientConnectionString
    {
        get
        {
            var variable = Environment.GetEnvironmentVariable("EMAIL_CLIENT_CONNECTION_STRING");
            return variable is null ? throw new EnvironmentVariableNotFoundException("EMAIL_CLIENT_CONNECTION_STRING") : variable;
        }

    }
    public static string BlobStorageConnectionString
    {
        get
        {
            var variable = Environment.GetEnvironmentVariable("BLOB_STORAGE_CONNECTION_STRING");
            return variable is null ? throw new EnvironmentVariableNotFoundException("BLOB_STORAGE_CONNECTION_STRING") : variable;
        }
    }
}
