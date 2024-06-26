using System.Data.Common;

namespace NotificationManager.Common.Helpers;
public static class ConnectionStringParseHelper
{
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
