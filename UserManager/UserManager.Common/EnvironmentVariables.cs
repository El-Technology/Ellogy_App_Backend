using UserManager.Common.Exceptions;

namespace UserManager.Common;

public static class EnvironmentVariables
{
	public static readonly string? ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") 
													  ?? throw new EnvironmentVariableNotFoundException("CONNECTION_STRING");
	
    public static readonly string? JwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") 
												  ?? throw new EnvironmentVariableNotFoundException("JWT_SECRET_KEY");
	
    public static readonly string? CommunicationServiceConnectionString = Environment.GetEnvironmentVariable("COMMUNICATION_SERVICE_CONNECTION_STRING") 
                                                                          ?? throw new EnvironmentVariableNotFoundException("COMMUNICATION_SERVICE_CONNECTION_STRING");
}
