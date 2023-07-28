namespace UserManager.BLL.Exceptions
{
    public class InvalidRefreshTokenException : Exception
    {
        public new string Message { get; set; } = "Refresh token is not valid";
        public InvalidRefreshTokenException() { }
    }
}
