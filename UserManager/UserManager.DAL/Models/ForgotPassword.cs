namespace UserManager.DAL.Models;

public class ForgotPassword
{
    public Guid Id { get; set; }
    public string Token { get; set; }
    public Guid UserId { get; set; }
    public DateTime ExpireDate { get; set; }
    public bool IsValid { get; set; }

    public ForgotPassword(string token, Guid userId, TimeSpan tokenTtl)
    {
        UserId = userId;
        Token = token;
        Id = Guid.NewGuid();
        ExpireDate = DateTime.UtcNow.Add(tokenTtl);
        IsValid = true;
    }
}
