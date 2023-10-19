namespace TicketsManager.Common
{
    public static class EnvironmentVariables
    {
        public static readonly string? ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                                                          ?? "default_CONNECTION_STRING";

        public static readonly string? JwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                                                      ?? "default_JWT_SECRET_KEY_HAVE_32_S";
    }
}