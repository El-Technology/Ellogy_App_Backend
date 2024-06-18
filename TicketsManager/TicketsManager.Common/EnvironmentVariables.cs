using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System.Collections.Concurrent;
using TicketsManager.Common.Constants;
using TicketsManager.Common.Helpers;

namespace TicketsManager.Common;

public static class EnvironmentVariables
{
    private static readonly Lazy<Task<ConcurrentDictionary<string, string>>> _secrets = new(async () =>
    {
        var vaultUri = new Uri(ConfigHelper.AppSetting(ConfigConstants.KeyVaultStorageUrl));
        var client = new SecretClient(vaultUri, new DefaultAzureCredential());
        var secretsDictionary = new ConcurrentDictionary<string, string>();

        await Task.WhenAll(
            GetAndAddSecretAsync(client, SecretNames.ConnectionString, secretsDictionary),
            GetAndAddSecretAsync(client, SecretNames.JwtSecretKey, secretsDictionary),
            GetAndAddSecretAsync(client, SecretNames.Host, secretsDictionary)
        );

        return secretsDictionary;
    });

    private static async Task GetAndAddSecretAsync(SecretClient client, string secretName, ConcurrentDictionary<string, string> secretsDictionary)
    {
        var secret = await client.GetSecretAsync(secretName);
        secretsDictionary.TryAdd(secretName, secret.Value.Value);
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

    public static Task<string> ConnectionString =>
        /*GetSecretAsync(SecretNames.ConnectionString)*/
        Task.FromResult("Server=localhost;Database=TicketManager;Port=5432;User Id=postgres;Password=password;");
    public static Task<string> JwtSecretKey =>
        /*GetSecretAsync(SecretNames.JwtSecretKey)*/
        Task.FromResult("testJWTtokenForLocalDevelopmentt");

    public static Task<string> Host => /*GetSecretAsync(SecretNames.Host)*/Task.FromResult("localhost");
}
