using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UserManager.Common.Constants;

namespace NotificationService;

public static class EnvironmentVariables
{
    private static readonly Lazy<Task<ConcurrentDictionary<string, string>>> _secrets = new(async () =>
    {
        var vaultUri = new Uri(Environment.GetEnvironmentVariable(ConfigConstants.KeyVaultStorageUrl));
        var client = new SecretClient(vaultUri, new DefaultAzureCredential());
        var secretsDictionary = new ConcurrentDictionary<string, string>();

        await Task.WhenAll(
            GetAndAddSecretAsync(client, SecretNames.MailFrom, secretsDictionary),
            GetAndAddSecretAsync(client, SecretNames.EmailClientConnectionString, secretsDictionary),
            GetAndAddSecretAsync(client, SecretNames.BlobStorageConnectionString, secretsDictionary),
            GetAndAddSecretAsync(client, SecretNames.AppCdnUrl, secretsDictionary),
            GetAndAddSecretAsync(client, SecretNames.ConsumerEmail, secretsDictionary)
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

    public static Task<string> AppCdnUrl => GetSecretAsync(SecretNames.AppCdnUrl);
    public static Task<string> ConsumerEmail => GetSecretAsync(SecretNames.ConsumerEmail);
    public static Task<string> MailFrom => GetSecretAsync(SecretNames.MailFrom);
    public static Task<string> EmailClientConnectionString => GetSecretAsync(SecretNames.EmailClientConnectionString);
    public static Task<string> BlobStorageConnectionString => GetSecretAsync(SecretNames.BlobStorageConnectionString);
}
