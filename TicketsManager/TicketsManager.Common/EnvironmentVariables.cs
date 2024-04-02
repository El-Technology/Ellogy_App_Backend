using TicketsManager.Common.Constants;
using TicketsManager.Common.Helpers;

namespace TicketsManager.Common;

public static class EnvironmentVariables
{
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
}