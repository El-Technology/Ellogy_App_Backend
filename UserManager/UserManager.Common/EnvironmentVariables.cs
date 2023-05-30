namespace UserManager.Common;

public static class EnvironmentVariables
{
    public static readonly string? ConnectionString = Environment.GetEnvironmentVariable("ConnectionString");
    public static readonly string? JwtSecretKey = Environment.GetEnvironmentVariable("JwtSecretKey");
}
