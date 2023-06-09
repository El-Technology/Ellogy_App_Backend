namespace OcelotApiGateway.Common;

public static class EnvironmentVariables
{
    public static readonly string? JwtSecretKey = Environment.GetEnvironmentVariable("JwtSecretKey");
}
