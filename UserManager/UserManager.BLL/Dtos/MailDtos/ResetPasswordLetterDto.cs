namespace UserManager.BLL.Dtos.MailDtos;

public class ResetPasswordLetterDto
{
    public ResetPasswordLetterDto(string resetPasswordUrl, string userEmail, string userName)
    {
        ResetPasswordUrl = resetPasswordUrl;
        UserEmail = userEmail;
        UserName = userName;
    }

    public string ResetPasswordUrl { get; set; }
    public string UserEmail { get; set; }
    public string UserName { get; set; }

}
