namespace UserManager.Common;

public static class EnvironmentVariables
{
    public static readonly string? ConnectionString = Environment.GetEnvironmentVariable("ConnectionString");
}
