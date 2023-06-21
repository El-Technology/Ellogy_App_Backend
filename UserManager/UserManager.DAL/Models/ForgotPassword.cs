namespace UserManager.DAL.Models;

public class ForgotPassword
{
    public string Token { get; set; }
    public Guid UserId { get; set; }
    public DateTime ExpireDate { get; set; }
    public bool IsValid { get; set; }

    public ForgotPassword() { }

    public ForgotPassword(string token, Guid userId, TimeSpan tokenTtl)
    {
        UserId = userId;
        Token = token;
        ExpireDate = DateTime.UtcNow.Add(tokenTtl);
        IsValid = true;
    }
}
