namespace TicketsManager.Common
{
    public static class EnvironmentVariables
    {
        public static readonly string? ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        public static readonly string? JwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
    }
}