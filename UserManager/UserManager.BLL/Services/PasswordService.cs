﻿using System.Web;
using UserManager.BLL.Dtos.PasswordDtos;
using UserManager.BLL.Exceptions;
using UserManager.BLL.Interfaces;
using UserManager.Common.Helpers;
using UserManager.Common.Models.NotificationModels;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.BLL.Services;

public class PasswordService : IPasswordService
{
    private readonly TimeSpan _tokenTtl = TimeSpan.FromDays(1);
    private readonly IUserRepository _userRepository;
    private readonly IForgotPasswordRepository _forgotPasswordRepository;
    private readonly INotificationQueueService _notificationQueueService;

    private readonly NotificationModel _notificationModel = new()
    {
        Type = NotificationTypeEnum.ResetPassword,
        Way = NotificationWayEnum.Email
    };
    private const string PasswordResetUrlTemplate = "{0}?id={1}&token={2}";
    private const string ResetPasswordPattern = "{{{resetPasswordLink}}}";

    public PasswordService(IUserRepository userRepository, IForgotPasswordRepository forgotPasswordRepository, INotificationQueueService notificationQueueService)
    {
        _userRepository = userRepository;
        _forgotPasswordRepository = forgotPasswordRepository;
        _notificationQueueService = notificationQueueService;
    }

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
        _notificationModel.MetaData = new() { { ResetPasswordPattern, resetPasswordUrl } };

        await _notificationQueueService.SendNotificationAsync(_notificationModel);
    }

    public async Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        resetPasswordDto.Token = HttpUtility.UrlDecode(resetPasswordDto.Token);

        if (!await _forgotPasswordRepository.ValidateResetRequestAsync(resetPasswordDto.Id, resetPasswordDto.Token))
            throw new PasswordResetFailedException();

        var user = await _userRepository.GetUserByForgetPasswordIdAsync(resetPasswordDto.Id) ?? throw new UserNotFoundException();

        var newSalt = CryptoHelper.GenerateSalt();
        var hashedNewPassword = CryptoHelper.GetHash(resetPasswordDto.Password, newSalt);

        user.Password = hashedNewPassword;
        user.Salt = newSalt;

        await _userRepository.UpdateUserAsync(user);

        await _forgotPasswordRepository.InvalidateTokenAsync(resetPasswordDto.Id);
    }
}
