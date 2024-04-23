using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using UserManager.Common.Constants;
using UserManager.Common.Helpers;

namespace UserManager.Common;

public static class EnvironmentVariables
{
    private static readonly Lazy<Task<Dictionary<string, string>>> _secrets = new(async () =>
    {
        var vaultUri = new Uri(ConfigHelper.AppSetting(ConfigConstants.KeyVaultStorageUrl));
        var client = new SecretClient(vaultUri, new DefaultAzureCredential());

        var secrets = new Dictionary<string, string>
        {
            { SecretNames.AzureServiceBusConnectionString, await GetSecretAsync(client, SecretNames.AzureServiceBusConnectionString) },
            { SecretNames.ConnectionString, await GetSecretAsync(client, SecretNames.ConnectionString) },
            { SecretNames.ConnectionStringPayment, await GetSecretAsync(client, SecretNames.ConnectionStringPayment) },
            { SecretNames.JwtSecretKey, await GetSecretAsync(client, SecretNames.JwtSecretKey) },
            { SecretNames.EmailClientConnectionString, await GetSecretAsync(client, SecretNames.EmailClientConnectionString) },
            { SecretNames.BlobStorageConnectionString, await GetSecretAsync(client, SecretNames.BlobStorageConnectionString) }
        };

        return secrets;
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

    public static Task<string> AzureServiceBusConnectionString => GetSecretAsync(SecretNames.AzureServiceBusConnectionString);
    public static Task<string> ConnectionString => GetSecretAsync(SecretNames.ConnectionString);
    public static Task<string> ConnectionStringPayment => GetSecretAsync(SecretNames.ConnectionStringPayment);
    public static Task<string> JwtSecretKey => GetSecretAsync(SecretNames.JwtSecretKey);
    public static Task<string> EmailClientConnectionString => GetSecretAsync(SecretNames.EmailClientConnectionString);
    public static Task<string> BlobStorageConnectionString => GetSecretAsync(SecretNames.BlobStorageConnectionString);
}