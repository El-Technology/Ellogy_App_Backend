using AICommunicationService.Common.Constants;
using AICommunicationService.Common.Helpers;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

public static class EnvironmentVariables
{
    private static readonly Lazy<Task<Dictionary<string, string>>> _secrets = new(async () =>
    {
        var vaultUri = new Uri(ConfigHelper.AppSetting(ConfigConstants.KeyVaultStorageUrl));
        var client = new SecretClient(vaultUri, new DefaultAzureCredential());

        return new Dictionary<string, string>
        {
            { SecretNames.ConnectionString, await GetSecretAsync(client, SecretNames.ConnectionString) },
            { SecretNames.GroqKey, await GetSecretAsync(client, SecretNames.GroqKey) },
            { SecretNames.OpenAiKey, await GetSecretAsync(client, SecretNames.OpenAiKey) },
            { SecretNames.JwtSecretKey, await GetSecretAsync(client, SecretNames.JwtSecretKey) },
            { SecretNames.ConnectionStringPayment, await GetSecretAsync(client, SecretNames.ConnectionStringPayment) },
            { SecretNames.ConnectionStringVector, await GetSecretAsync(client, SecretNames.ConnectionStringVector) },
            { SecretNames.BlobStorageConnectionString, await GetSecretAsync(client, SecretNames.BlobStorageConnectionString) }
        };
    });

    private static async Task<string> GetSecretAsync(SecretClient client, string secretName)
    {
        var secret = await client.GetSecretAsync(secretName);
        return secret.Value.Value;
    }

    public static async Task<string> GetSecretAsync(string key)
    {
        var secrets = await _secrets.Value;
        if (!secrets.ContainsKey(key))
        {
            throw new ArgumentException($"Secret with key '{key}' not found");
        }

        return secrets[key];
    }

    public static Task<string> GetConnectionStringAsync => GetSecretAsync(SecretNames.ConnectionString);
    public static Task<string> GetGroqKeyAsync => GetSecretAsync(SecretNames.GroqKey);
    public static Task<string> GetOpenAiKeyAsync => GetSecretAsync(SecretNames.OpenAiKey);
    public static Task<string> GetJwtSecretKeyAsync => GetSecretAsync(SecretNames.JwtSecretKey);
    public static Task<string> GetConnectionStringPaymentAsync => GetSecretAsync(SecretNames.ConnectionStringPayment);
    public static Task<string> GetConnectionStringVectorAsync => GetSecretAsync(SecretNames.ConnectionStringVector);
    public static Task<string> GetBlobStorageConnectionStringAsync => GetSecretAsync(SecretNames.BlobStorageConnectionString);
}