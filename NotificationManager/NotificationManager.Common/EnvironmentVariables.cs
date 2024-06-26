using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using NotificationManager.Common.Constants;
using NotificationManager.Common.Helpers;
using System.Collections.Concurrent;

namespace NotificationManager.Common;

public static class EnvironmentVariables
{
    private static readonly Lazy<Task<ConcurrentDictionary<string, string>>> _secrets = new(async () =>
    {
        var vaultUri = new Uri(ConfigHelper.AppSetting(ConfigConstants.KeyVaultStorageUrl) ?? string.Empty);
        var client = new SecretClient(vaultUri, new DefaultAzureCredential());
        var secretsDictionary = new ConcurrentDictionary<string, string>();

        await GetAndAddSecretAsync(client, SecretNames.JwtSecretKey, secretsDictionary);
        await GetAndAddSecretAsync(client, SecretNames.MailFrom, secretsDictionary);
        await GetAndAddSecretAsync(client, SecretNames.AppCdnUrl, secretsDictionary);
        await GetAndAddSecretAsync(client, SecretNames.ConsumerEmail, secretsDictionary);
        await GetAndAddSecretAsync(client, SecretNames.BlobStorageConnectionString, secretsDictionary);
        await GetAndAddSecretAsync(client, SecretNames.AzureServiceBusConnectionString, secretsDictionary);
        await GetAndAddSecretAsync(client, SecretNames.EmailClientConnectionString, secretsDictionary);


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
        if (!secrets.TryGetValue(key, out var value))
            throw new ArgumentException($"Secret with key '{key}' not found");

        return value;
    }


    public static Task<string> JwtSecretKey => GetSecretAsync(SecretNames.JwtSecretKey);
    public static Task<string> MailFrom => GetSecretAsync(SecretNames.MailFrom);
    public static Task<string> AppCdnUrl => GetSecretAsync(SecretNames.AppCdnUrl);
    public static Task<string> ConsumerEmail => GetSecretAsync(SecretNames.ConsumerEmail);
    public static Task<string> BlobStorageConnectionString => GetSecretAsync(SecretNames.BlobStorageConnectionString);
    public static Task<string> AzureServiceBusConnectionString => GetSecretAsync(SecretNames.AzureServiceBusConnectionString);
    public static Task<string> EmailClientConnectionString => GetSecretAsync(SecretNames.EmailClientConnectionString);
}