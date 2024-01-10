namespace UserManager.BLL.Exceptions
{
    public class EmailVerificationException : Exception
    {
        public new string Message { get; set; } = "You need to activate your account";
        public EmailVerificationException()
        { }
    }
}
