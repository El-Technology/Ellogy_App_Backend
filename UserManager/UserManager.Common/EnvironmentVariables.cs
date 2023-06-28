namespace UserManager.Common;

public static class EnvironmentVariables
{
    //TODO add exceptions
    public static readonly string? ConnectionString = Environment.GetEnvironmentVariable("ConnectionString");
    public static readonly string? JwtSecretKey = Environment.GetEnvironmentVariable("JwtSecretKey");
    public static readonly string? CommunicationServiceConnectionString = Environment.GetEnvironmentVariable("CommunicationServiceConnectionString");
}
