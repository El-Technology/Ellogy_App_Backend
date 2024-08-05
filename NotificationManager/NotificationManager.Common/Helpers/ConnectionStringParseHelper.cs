using System.Data.Common;

namespace NotificationManager.Common.Helpers;

/// <summary>
/// Helper class for parsing connection strings
/// </summary>
public static class ConnectionStringParseHelper
{
    /// <summary>
    /// Get a part of the connection string
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="keyWord"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static string? GetPartOfConnectionString(string connectionString, string keyWord)
    {
        var builder = new DbConnectionStringBuilder
        {
            ConnectionString = connectionString
        };

        if (!builder.TryGetValue(keyWord, out var blobEndpoint))
        {
            throw new InvalidOperationException($"{keyWord} not found in the connection string.");
        }

        return blobEndpoint.ToString();
    }
}
