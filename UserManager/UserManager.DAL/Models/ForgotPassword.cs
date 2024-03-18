namespace UserManager.DAL.Models;

public class ForgotPassword
{
    public Guid Id { get; set; }
    public string Token { get; set; }
    public DateTime ExpireDate { get; set; }
    public bool IsValid { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }

    public ForgotPassword() { }

    public ForgotPassword(string token, Guid userId, TimeSpan tokenTtl)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Token = token;
        ExpireDate = DateTime.UtcNow.Add(tokenTtl);
        IsValid = true;
    }
}
