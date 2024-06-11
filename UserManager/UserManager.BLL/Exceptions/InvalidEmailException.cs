namespace UserManager.BLL.Exceptions;
public class InvalidEmailException : Exception
{
    private const string MessageTemplate = "Email {0} is not valid";
    public new string Message { get; set; }

    public InvalidEmailException() : this(string.Empty)
    { }

    public InvalidEmailException(string email)
    {
        Message = string.Format(MessageTemplate, email);
    }
}
