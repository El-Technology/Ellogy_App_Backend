using AutoMapper;
using Azure.Storage.Blobs;
using System.Web;
using UserManager.BLL.Dtos.ProfileDto;
using UserManager.BLL.Dtos.RegisterDtos;
using UserManager.BLL.Exceptions;
using UserManager.BLL.Helpers;
using UserManager.BLL.Interfaces;
using UserManager.Common.Constants;
using UserManager.Common.Helpers;
using UserManager.Common.Models.AvatarImage;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.BLL.Services;

/// <summary>
///     Service for user profile operations
/// </summary>
public class UserProfileService : IUserProfileService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IMapper _mapper;
    private readonly IRegisterService _registerService;
    private readonly IUserRepository _userRepository;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="userRepository"></param>
    /// <param name="mapper"></param>
    /// <param name="blobServiceClient"></param>
    /// <param name="registerService"></param>
    public UserProfileService(IUserRepository userRepository, IMapper mapper, BlobServiceClient blobServiceClient,
        IRegisterService registerService)
    {
        _blobServiceClient = blobServiceClient;
        _mapper = mapper;
        _userRepository = userRepository;
        _registerService = registerService;
    }

    /// <inheritdoc cref="IUserProfileService.UpdateUserProfileAsync(Guid, UserProfileDto, Guid)" />
    public async Task<UserProfileDto> UpdateUserProfileAsync(Guid userId, UserProfileDto userProfileDto,
        Guid idFromToken)
    {
        ValidateUserAccess(userId, idFromToken);
        var user = await GetUserByIdAsync(userId);
        var updatedUser = _mapper.Map(userProfileDto, user);
        await _userRepository.UpdateUserAsync(updatedUser);
        return userProfileDto;
    }

    /// <inheritdoc cref="IUserProfileService.DeleteUserProfileAsync(Guid, Guid)" />
    public async Task<bool> DeleteUserProfileAsync(Guid userId, Guid idFromToken)
    {
        ValidateUserAccess(userId, idFromToken);
        var user = await GetUserByIdAsync(userId);
        await _userRepository.DeleteUserAsync(user);
        return true;
    }

    /// <inheritdoc cref="IUserProfileService.ChangeUserPasswordAsync(Guid, ChangePasswordDto, Guid)" />
    public async Task<bool> ChangeUserPasswordAsync(Guid userId, ChangePasswordDto changePasswordDto, Guid idFromToken)
    {
        ValidateUserAccess(userId, idFromToken);
        var user = await GetUserByIdAsync(userId);

        if (!CryptoHelper.ConfirmPassword(changePasswordDto.oldPassword, user.Salt, user.Password))
            throw new Exception("The old password is incorrect");

        var newSalt = CryptoHelper.GenerateSalt();
        var hashedNewPassword = CryptoHelper.GetHash(changePasswordDto.newPassword, newSalt);

        user.Password = hashedNewPassword;
        user.Salt = newSalt;

        await _userRepository.UpdateUserAsync(user);

        return true;
    }

    /// <inheritdoc cref="IUserProfileService.UploadUserAvatarAsync(UploadAvatar, Guid)" />
    public async Task<string> UploadUserAvatarAsync(UploadAvatar uploadAvatar, Guid idFromToken)
    {
        ValidateUserAccess(uploadAvatar.UserId, idFromToken);
        var user = await GetUserByIdAsync(uploadAvatar.UserId);

        var containerClient = _blobServiceClient.GetBlobContainerClient(BlobContainerConstants.AvatarsContainer);
        var bytes = Convert.FromBase64String(uploadAvatar.Base64Avatar);
        var fileName = $"{Guid.NewGuid()}{ImageExtensionHelper.GetImageExtention(uploadAvatar.ImageExtension)}";
        var blobClient = containerClient.GetBlobClient(fileName);
        using var memoryStream = new MemoryStream(bytes);
        await blobClient.UploadAsync(memoryStream, true);

        if (user.AvatarLink is not null)
        {
            var blobDeleteClient = containerClient.GetBlobClient(user.AvatarLink);
            await blobDeleteClient.DeleteIfExistsAsync();
        }

        var blobUri = blobClient.Uri.ToString();
        user.AvatarLink = blobUri;
        await _userRepository.UpdateUserAsync(user);

        return blobUri;
    }

    public async Task<GetUserProfileDto> GetUserProfileAsync(Guid userId)
    {
        var user = await GetUserByIdAsync(userId);
        var userProfileDto = _mapper.Map<GetUserProfileDto>(user);

        userProfileDto.Jwt = await JwtHelper.GenerateJwtAsync(user);

        return userProfileDto;
    }

    /// <inheritdoc cref="IUserProfileService.ChangeUserEmailAsync(Guid, SendVerificationEmailDto)" />
    public async Task ChangeUserEmailAsync(Guid userId, SendVerificationEmailDto sendVerificationEmailDto)
    {
        if (!EmailHelper.IsValidEmail(sendVerificationEmailDto.UserEmail))
            throw new InvalidEmailException();

        if (await _userRepository.CheckEmailIsExistAsync(sendVerificationEmailDto.UserEmail))
            throw new UserAlreadyExistException(sendVerificationEmailDto.UserEmail);

        var user = await GetUserByIdAsync(userId)
                   ?? throw new UserNotFoundException($"User with id => {userId} was not found");

        user.VerifyToken = CryptoHelper.GenerateToken();
        await _userRepository.UpdateUserAsync(user);

        user.Email = sendVerificationEmailDto.UserEmail;

        await _registerService.SendVerificationEmailAsync(sendVerificationEmailDto, user);
    }

    /// <inheritdoc cref="IUserProfileService.VerifyUserEmailAsync(Guid, ActivateUserAccountDto)" />
    public async Task VerifyUserEmailAsync(Guid userId, ActivateUserAccountDto activateUser)
    {
        if (!EmailHelper.IsValidEmail(activateUser.UserEmail))
            throw new InvalidEmailException();

        if (await _userRepository.CheckEmailIsExistAsync(activateUser.UserEmail))
            throw new UserAlreadyExistException(activateUser.UserEmail);

        activateUser.Token = HttpUtility.UrlDecode(activateUser.Token);

        var user = await _userRepository.GetUserByIdAsync(userId)
                   ?? throw new UserNotFoundException();

        if (user.VerifyToken is null)
            throw new Exception("Verify token was not found");

        if (CryptoHelper.GetHash(user.VerifyToken) != activateUser.Token)
            throw new Exception("Wrong activate token");

        user.Email = activateUser.UserEmail;

        await _userRepository.UpdateUserAsync(user);
    }

    /// <summary>
    ///     This method compare input user id and another one from JWT token
    /// </summary>
    /// <param name="inputId"></param>
    /// <param name="idFromToken"></param>
    /// <exception cref="Exception"></exception>
    private void ValidateUserAccess(Guid inputId, Guid idFromToken)
    {
        if (inputId != idFromToken)
            throw new Exception("You don`t have access to another user data");
    }

    /// <summary>
    ///     This method retrieves the user by id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="UserNotFoundException"></exception>
    private async Task<User> GetUserByIdAsync(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId)
                   ?? throw new UserNotFoundException($"User with id => {userId} was not found");

        return user;
    }
}