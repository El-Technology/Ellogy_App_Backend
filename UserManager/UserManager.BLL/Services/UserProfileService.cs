using AutoMapper;
using Azure.Storage.Blobs;
using UserManager.BLL.Dtos.ProfileDto;
using UserManager.BLL.Exceptions;
using UserManager.BLL.Helpers;
using UserManager.BLL.Interfaces;
using UserManager.Common.Constants;
using UserManager.Common.Helpers;
using UserManager.Common.Models.AvatarImage;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.BLL.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserProfileService(IUserRepository userRepository, IMapper mapper, BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        /// <summary>
        /// This method compare input user id and another one from JWT token
        /// </summary>
        /// <param name="inputId"></param>
        /// <param name="idFromToken"></param>
        /// <exception cref="Exception"></exception>
        private void ValidateUserAccess(Guid inputId, Guid idFromToken)
        {
            if (inputId != idFromToken)
                throw new Exception("You don`t have access to another user data");
        }

        private async Task<User> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new UserNotFoundException($"User with id => {userId} was not found");

            return user;
        }

        /// <inheritdoc cref="IUserProfileService.UpdateUserProfileAsync(Guid, UserProfileDto, Guid)"/>
        public async Task<UserProfileDto> UpdateUserProfileAsync(Guid userId, UserProfileDto userProfileDto, Guid idFromToken)
        {
            ValidateUserAccess(userId, idFromToken);
            var user = await GetUserByIdAsync(userId);
            var updatedUser = _mapper.Map(userProfileDto, user);
            await _userRepository.UpdateUserAsync(updatedUser);
            return userProfileDto;
        }

        /// <inheritdoc cref="IUserProfileService.DeleteUserProfileAsync(Guid, Guid)"/>
        public async Task<bool> DeleteUserProfileAsync(Guid userId, Guid idFromToken)
        {
            ValidateUserAccess(userId, idFromToken);
            var user = await GetUserByIdAsync(userId);
            await _userRepository.DeleteUserAsync(user);
            return true;
        }

        /// <inheritdoc cref="IUserProfileService.ChangeUserPasswordAsync(Guid, ChangePasswordDto, Guid)"/>
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

        /// <inheritdoc cref="IUserProfileService.UploadUserAvatarAsync(UploadAvatar, Guid)"/>
        public async Task<string> UploadUserAvatarAsync(UploadAvatar uploadAvatar, Guid idFromToken)
        {
            ValidateUserAccess(uploadAvatar.UserId, idFromToken);
            var user = await GetUserByIdAsync(uploadAvatar.UserId);

            var containerClient = _blobServiceClient.GetBlobContainerClient(BlobContainerConstants.AvatarsContainer);
            var bytes = Convert.FromBase64String(uploadAvatar.Base64Avatar);
            var fileName = $"{uploadAvatar.UserId}{ImageExtensionHelper.GetImageExtention(uploadAvatar.ImageExtension)}";
            var blobClient = containerClient.GetBlobClient(fileName);
            using var memoryStream = new MemoryStream(bytes);
            await blobClient.UploadAsync(memoryStream, overwrite: true);
            var blobUri = blobClient.Uri.ToString();

            user.AvatarLink = blobUri;
            await _userRepository.UpdateUserAsync(user);
            return blobUri;
        }
    }
}
