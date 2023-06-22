using UserManager.BLL.Dtos.MailDtos;

namespace UserManager.BLL.Interfaces;

public interface IMailService
{
    public Task SendPasswordResetLetterAsync(ResetPasswordLetterDto resetPasswordLetterDto);
}
