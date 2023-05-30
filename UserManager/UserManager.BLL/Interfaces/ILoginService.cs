namespace UserManager.BLL.Interfaces;

public interface ILoginService
{
    public Task<string> LoginUser(string email, string password);
}
