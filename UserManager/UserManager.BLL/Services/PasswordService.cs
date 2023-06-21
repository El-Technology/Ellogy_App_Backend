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
    private readonly IMailService _mailService;

    public PasswordService(IUserRepository userRepository, IForgotPasswordRepository forgotPasswordRepository, IMailService mailService)
    {
        _userRepository = userRepository;
        _forgotPasswordRepository = forgotPasswordRepository;
        _mailService = mailService;
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
        await _mailService.SendPasswordResetLetterAsync("google.com", forgotPasswordDto.Email, "Andrew");
    }

    public async Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        resetPasswordDto.Token = HttpUtility.UrlDecode(resetPasswordDto.Token);
        if (!await _forgotPasswordRepository.ValidateResetRequestAsync(resetPasswordDto.UserId, resetPasswordDto.Token))
            throw new PasswordResetFailedException();

        var user = await _userRepository.GetUserByIdAsync(resetPasswordDto.UserId) ?? throw new UserNotFoundException();

        var newSalt = CryptoHelper.GenerateSalt();
        var hashedNewPassword = CryptoHelper.GetHashPassword(resetPasswordDto.Password, newSalt);

        user.Password = hashedNewPassword;
        user.Salt = newSalt;

        await _userRepository.UpdateUserAsync(user);

        await _forgotPasswordRepository.InvalidateTokenAsync(resetPasswordDto.Token, resetPasswordDto.UserId);
    }
}
