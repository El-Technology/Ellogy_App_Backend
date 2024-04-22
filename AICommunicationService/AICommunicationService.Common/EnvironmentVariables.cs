namespace AICommunicationService.Common;

public static class EnvironmentVariables
{
    public static readonly string? GroqKey = Environment.GetEnvironmentVariable("GROQ_KEY")
    /*?? throw new EnvironmentVariableNotFoundException("OPEN_AI_KEY")*/;

    public static readonly string? OpenAiKey = Environment.GetEnvironmentVariable("OPEN_AI_KEY")
        /*?? throw new EnvironmentVariableNotFoundException("OPEN_AI_KEY")*/;

    public static readonly string? JwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
        /*?? throw new EnvironmentVariableNotFoundException("JWT_SECRET_KEY")*/;

    public static readonly string ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
        /* ?? throw new EnvironmentVariableNotFoundException("CONNECTION_STRING")*/;

    public static readonly string ConnectionStringPayment =
            Environment.GetEnvironmentVariable("CONNECTIONSTRING_PAYMENT")
        /*?? throw new EnvironmentVariableNotFoundException("CONNECTIONSTRING_PAYMENT")*/;

    public static readonly string ConnectionStringVector = Environment.GetEnvironmentVariable("CONNECTIONSTRING_VECTOR")
        /*  ?? throw new EnvironmentVariableNotFoundException("CONNECTIONSTRING_VECTOR")*/;

    public static readonly string BlobStorageConnectionString =
            Environment.GetEnvironmentVariable("BLOB_STORAGE_CONNECTION_STRING")
        /* ?? throw new EnvironmentVariableNotFoundException("BLOB_STORAGE_CONNECTION_STRING")*/;
}