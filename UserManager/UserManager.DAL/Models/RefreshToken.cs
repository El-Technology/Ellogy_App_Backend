namespace UserManager.DAL.Models;

public class RefreshToken
{
    public Guid Id { get; set; }

    public DateTime ExpireDate { get; set; }
    public string Value { get; set; }
    public bool IsValid { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }
}
