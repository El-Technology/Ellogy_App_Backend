namespace UserManager.Common;

public static class EnvironmentVariables
{
    //TODO add exceptions
    public static readonly string? ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
    public static readonly string? JwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
    public static readonly string? CommunicationServiceConnectionString = Environment.GetEnvironmentVariable("COMMUNICATION_SERVICE_CONNECTION_STRING");
}
