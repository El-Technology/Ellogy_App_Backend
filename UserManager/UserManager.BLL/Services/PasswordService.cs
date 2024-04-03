using System.Web;
using UserManager.BLL.Dtos.PasswordDtos;
using UserManager.BLL.Exceptions;
using UserManager.BLL.Interfaces;
using UserManager.Common.Helpers;
using UserManager.Common.Models.NotificationModels;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.BLL.Services;

/// <summary>
///     Service for password operations
/// </summary>
public class PasswordService : IPasswordService
{
    private const string PasswordResetUrlTemplate = "{0}?id={1}&token={2}";
    private const string ResetPasswordPattern = "{{{resetPasswordLink}}}";
    private readonly TimeSpan _tokenTtl = TimeSpan.FromDays(1);

    private readonly NotificationModel _notificationModel = new()
    {
        Type = NotificationTypeEnum.ResetPassword,
        Way = NotificationWayEnum.Email
    };

    private readonly IForgotPasswordRepository _forgotPasswordRepository;
    private readonly INotificationQueueService _notificationQueueService;
    private readonly IUserRepository _userRepository;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="userRepository"></param>
    /// <param name="forgotPasswordRepository"></param>
    /// <param name="notificationQueueService"></param>
    public PasswordService(IUserRepository userRepository, IForgotPasswordRepository forgotPasswordRepository,
        INotificationQueueService notificationQueueService)
    {
        _userRepository = userRepository;
        _forgotPasswordRepository = forgotPasswordRepository;
        _notificationQueueService = notificationQueueService;
    }

    /// <summary>
    ///     Initiates the Forgot Password process.
    /// </summary>
    /// <param name="forgotPasswordDto"></param>
    /// <returns></returns>
    /// <exception cref="UserNotFoundException"></exception>
    public async Task ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
    {
        if (!await _userRepository.CheckEmailIsExistAsync(forgotPasswordDto.Email))
            throw new UserNotFoundException(forgotPasswordDto.Email);

        var token = CryptoHelper.GenerateToken();
        var user = (await _userRepository.GetUserByEmailAsync(forgotPasswordDto.Email))!;

        var forgotPasswordEntry = new ForgotPassword(token, user.Id, _tokenTtl);
        await _forgotPasswordRepository.AddForgotTokenAsync(forgotPasswordEntry);

        var hashToken = CryptoHelper.GetHash(token);
        var resetPasswordUrl = string.Format(PasswordResetUrlTemplate, forgotPasswordDto.RedirectUrl,
            HttpUtility.UrlEncode(forgotPasswordEntry.Id.ToString()), HttpUtility.UrlEncode(hashToken));


        _notificationModel.Consumer = user.Email;
        _notificationModel.MetaData = new Dictionary<string, string> { { ResetPasswordPattern, resetPasswordUrl } };

        await _notificationQueueService.SendNotificationAsync(_notificationModel);
    }

    /// <summary>
    ///     Resets the user's password.
    /// </summary>
    /// <param name="resetPasswordDto"></param>
    /// <returns></returns>
    /// <exception cref="PasswordResetFailedException"></exception>
    /// <exception cref="UserNotFoundException"></exception>
    public async Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        resetPasswordDto.Token = HttpUtility.UrlDecode(resetPasswordDto.Token);

        if (!await _forgotPasswordRepository.ValidateResetRequestAsync(resetPasswordDto.Id, resetPasswordDto.Token))
            throw new PasswordResetFailedException();

        var user = await _userRepository.GetUserByForgetPasswordIdAsync(resetPasswordDto.Id) ??
                   throw new UserNotFoundException();

        var newSalt = CryptoHelper.GenerateSalt();
        var hashedNewPassword = CryptoHelper.GetHash(resetPasswordDto.Password, newSalt);

        user.Password = hashedNewPassword;
        user.Salt = newSalt;

        await _userRepository.UpdateUserAsync(user);

        await _forgotPasswordRepository.InvalidateTokenAsync(resetPasswordDto.Id);
    }
}