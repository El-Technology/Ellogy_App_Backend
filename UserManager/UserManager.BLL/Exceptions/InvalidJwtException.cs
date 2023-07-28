namespace UserManager.BLL.Exceptions
{
    public class InvalidJwtException : Exception
    {
        public new string Message { get; set; } = "Jwt token is not valid";
        public InvalidJwtException() { }
    }
}
