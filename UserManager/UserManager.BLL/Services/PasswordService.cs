using System.Web;
using UserManager.BLL.Dtos.PasswordDtos;
using UserManager.BLL.Exceptions;
using UserManager.BLL.Helpers;
using UserManager.BLL.Interfaces;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.BLL.Services;

public class PasswordService : IPasswordService
{
    private readonly TimeSpan _tokenTtl = TimeSpan.FromDays(1);
    private readonly IUserRepository _userRepository;
    private readonly IForgotPasswordRepository _forgotPasswordRepository;

    public PasswordService(IUserRepository userRepository, IForgotPasswordRepository forgotPasswordRepository)
    {
        _userRepository = userRepository;
        _forgotPasswordRepository = forgotPasswordRepository;
    }

    public async Task ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
    {
        if (!(await _userRepository.CheckEmailIsExistAsync(forgotPasswordDto.Email)))
            throw new UserNotFoundException(forgotPasswordDto.Email);

        var token = CryptoHelper.GenerateToken();
        var userId = (await _userRepository.GetUserByEmailAsync(forgotPasswordDto.Email))!.Id;

        var forgotPasswordEntry = new ForgotPassword(token, userId, _tokenTtl);
        await _forgotPasswordRepository.AddForgotTokenAsync(forgotPasswordEntry);

        var resetPasswordUrl = $"{forgotPasswordDto.RedirectUrl}?$userId={HttpUtility.UrlEncode(userId.ToString())}&token={HttpUtility.UrlEncode(token)}";
    }

    public async Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        if (!await _forgotPasswordRepository.ValidateResetRequestAsync(resetPasswordDto.UserId, resetPasswordDto.Token))
            throw new PasswordResetFailedException();


    }
}
