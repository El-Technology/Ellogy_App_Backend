namespace AICommunicationService.BLL.Constants;

public static class AzureAiConstants
{
    public const string TurboModel = "EllogyGptTurbo";
    public const string FourModel = "EllogyGptFour";
    public const string FourTurboModel = "EllogyGptFourTurbo";
    public const string Four32kModel = "EllogyGptFour32k";
    public const string ResourceName = "EllogyCommunication";
    public const string EmbeddingUrl = $"https://ellogycommunication.openai.azure.com/openai/deployments/EllogyEmbedding3Small/embeddings?{ApiVersion}";
    public const string ApiVersion = "api-version=2024-02-15-preview";
    public const string BaseUrl = "https://ellogycommunication.openai.azure.com/openai/deployments/";
}
