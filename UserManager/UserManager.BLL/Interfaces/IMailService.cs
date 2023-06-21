namespace UserManager.BLL.Interfaces;

public interface IMailService
{
    public Task SendPasswordResetLetterAsync(string resetPasswordUrl, string receiverEmail, string name);
}
