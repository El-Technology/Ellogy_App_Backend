using AICommunicationService.Common;

namespace AICommunicationService.BLL.Constants;

public static class AzureAiConstants
{
    public const string TurboModel = "EllogyGptTurbo";
    public const string FourModel = "EllogyGptFour";
    public const string ApiVersion = "api-version=2024-02-15-preview";
    public static string BaseUrl = $"{EnvironmentVariables.AzureOpenAiBaseUrl}/openai/deployments/";
}
