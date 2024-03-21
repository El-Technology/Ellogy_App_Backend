using AICommunicationService.Common.Constants;
using AICommunicationService.Common.Exceptions;

namespace AICommunicationService.Common;

public static class EnvironmentVariables
{
    public static string AzureOpenAiBaseUrl = Environment.GetEnvironmentVariable("AZURE_OPEN_AI_BASE_URL")
                                                      ?? throw new EnvironmentVariableNotFoundException("AZURE_OPEN_AI_BASE_URL");

    public static readonly string? OpenAiKey = Environment.GetEnvironmentVariable("OPEN_AI_KEY")
                                                      ?? throw new EnvironmentVariableNotFoundException("OPEN_AI_KEY");

    public static readonly string? JwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                                                  ?? throw new EnvironmentVariableNotFoundException("JWT_SECRET_KEY");

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
}