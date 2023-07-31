using TicketsManager.Common.Exceptions;

namespace TicketsManager.Common
{
    public static class EnvironmentVariables
    {
        public static readonly string? ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                                                          ?? throw new EnvironmentVariableNotFoundException("CONNECTION_STRING");
		
        public static readonly string? JwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                                                      ?? throw new EnvironmentVariableNotFoundException("JWT_SECRET_KEY");

    }
}