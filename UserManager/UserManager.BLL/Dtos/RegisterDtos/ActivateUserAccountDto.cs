using UserManager.Common.Attributes;

namespace UserManager.BLL.Dtos.RegisterDtos;

public class ActivateUserAccountDto
{
    public string Token { get; set; }

    [EmailValidation]
    public string UserEmail { get; set; }
}
