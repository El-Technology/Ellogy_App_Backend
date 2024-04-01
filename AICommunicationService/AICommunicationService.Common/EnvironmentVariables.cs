using AICommunicationService.Common.Constants;
using AICommunicationService.Common.Helpers;

namespace AICommunicationService.Common;

public static class EnvironmentVariables
{
    public static readonly string? OpenAiKey = Environment.GetEnvironmentVariable("OPEN_AI_KEY")
        /*?? throw new EnvironmentVariableNotFoundException("OPEN_AI_KEY")*/;

    public static readonly string? JwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
        /*?? throw new EnvironmentVariableNotFoundException("JWT_SECRET_KEY")*/;

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

    public static bool EnablePayments = bool.Parse(Environment.GetEnvironmentVariable("ENABLE_PAYMENTS"));

    public static string Host = Environment.GetEnvironmentVariable("SSH_HOST")
        /*?? throw new EnvironmentVariableNotFoundException("HOST")*/;

    public static readonly string BlobStorageConnectionString =
            Environment.GetEnvironmentVariable("BLOB_STORAGE_CONNECTION_STRING")
        /* ?? throw new EnvironmentVariableNotFoundException("BLOB_STORAGE_CONNECTION_STRING")*/;
}