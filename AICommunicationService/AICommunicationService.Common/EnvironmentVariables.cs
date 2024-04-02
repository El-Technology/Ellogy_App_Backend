using AICommunicationService.Common.Constants;
using AICommunicationService.Common.Helpers;

namespace AICommunicationService.Common;

public static class EnvironmentVariables
{
    public static string OpenAiKey
    {
        get
        {
            var variable = Environment.GetEnvironmentVariable("OPEN_AI_KEY");
            return variable is null
                ? variable = "default_OPEN_AI_KEY"
                : variable;
        }
    }

    public static string JwtSecretKey
    {
        get
        {
            var variable = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            return variable is null
                ? variable = "default_JWT_SECRET_KEY_HAVE_32_S"
                : variable;
        }
    }

    public static string ConnectionString
    {
        get
        {
            var variable = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            return variable is null
                ? variable = "default_CONNECTION_STRING"
                : variable.Replace(
                    ConfigConstants.DbReplacePattern,
                    ConfigHelper.AppSetting(ConfigConstants.DbName));
        }
    }

    public static bool EnablePayments
    {
        get
        {
            var variable = Environment.GetEnvironmentVariable("ENABLE_PAYMENTS");
            return variable is null
                ? false
                : bool.Parse(variable);
        }
    }

    public static string Host
    {
        get
        {
            var variable = Environment.GetEnvironmentVariable("SSH_HOST");
            return variable is null
                ? variable = "default_host"
                : variable;
        }
    }

    public static string BlobStorageConnectionString
    {
        get
        {
            var variable = Environment.GetEnvironmentVariable("BLOB_STORAGE_CONNECTION_STRING");
            return variable is null
                ? variable = "default_BLOB_STORAGE_CONNECTION_STRING"
                : variable;
        }
    }
}