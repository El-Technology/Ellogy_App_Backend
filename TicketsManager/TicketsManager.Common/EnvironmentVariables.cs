using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using TicketsManager.Common.Constants;
using TicketsManager.Common.Helpers;

namespace TicketsManager.Common;

public static class EnvironmentVariables
{

    private static readonly Lazy<Task<Dictionary<string, string>>> _secrets = new(async () =>
    {
        var vaultUri = new Uri(ConfigHelper.AppSetting(ConfigConstants.KeyVaultStorageUrl));
        var client = new SecretClient(vaultUri, new DefaultAzureCredential());

        return new Dictionary<string, string>
        {
        { SecretNames.ConnectionString, await GetSecretAsync(client, SecretNames.ConnectionString) },
        { SecretNames.JwtSecretKey, await GetSecretAsync(client, SecretNames.JwtSecretKey) }
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

    public static Task<string> ConnectionString => GetSecretAsync(SecretNames.ConnectionString);
    public static Task<string> JwtSecretKey => GetSecretAsync(SecretNames.JwtSecretKey);
}