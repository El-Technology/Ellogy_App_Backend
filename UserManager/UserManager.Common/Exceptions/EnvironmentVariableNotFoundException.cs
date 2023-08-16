namespace UserManager.Common.Exceptions;

public class EnvironmentVariableNotFoundException : Exception
{
    private const string MessageTemplate = "Environment variable \"{0}\" not found ";
    public new string Message { get; set; }
    public EnvironmentVariableNotFoundException(string variableName)
    {
        Message = string.Format(MessageTemplate, variableName);
    }
}
