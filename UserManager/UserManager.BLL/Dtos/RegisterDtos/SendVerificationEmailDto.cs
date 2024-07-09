using UserManager.Common.Attributes;

namespace UserManager.BLL.Dtos.RegisterDtos;

public class SendVerificationEmailDto
{
    public string RedirectLink { get; set; }

    [EmailValidation]
    public string UserEmail { get; set; }
}
