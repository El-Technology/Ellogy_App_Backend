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

        private async Task<User> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new UserNotFoundException($"User with id => {userId} was not found");

            return user;
        }

        /// <inheritdoc cref="IUserProfileService.UpdateUserProfileAsync(Guid, UserProfileDto)"/>
        public async Task<UserProfileDto> UpdateUserProfileAsync(Guid userId, UserProfileDto userProfileDto)
        {
            var user = await GetUserByIdAsync(userId);
            var updatedUser = _mapper.Map(userProfileDto, user);
            await _userRepository.UpdateUserAsync(updatedUser);
            return userProfileDto;
        }

        /// <inheritdoc cref="IUserProfileService.DeleteUserProfileAsync(Guid)"/>
        public async Task<bool> DeleteUserProfileAsync(Guid userId)
        {
            var user = await GetUserByIdAsync(userId);
            await _userRepository.DeleteUserAsync(user);
            return true;
        }

        /// <inheritdoc cref="IUserProfileService.ChangeUserPasswordAsync(Guid, string)"/>
        public async Task<bool> ChangeUserPasswordAsync(Guid userId, ChangePasswordDto changePasswordDto)
        {
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

        /// <inheritdoc cref="IUserProfileService.UploadUserAvatarAsync(UploadAvatar)"/>
        public async Task<string> UploadUserAvatarAsync(UploadAvatar uploadAvatar)
        {
            var user = await GetUserByIdAsync(uploadAvatar.UserId);

            var containerClient = _blobServiceClient.GetBlobContainerClient(BlobContainerConstants.AvatarsContainer);
            var bytes = Convert.FromBase64String(uploadAvatar.Base64Avatar);
            var fileName = $"{uploadAvatar.UserId}{ImageExtensionHelper.GetImageExtention(uploadAvatar.ImageExtension)}";
            var blobClient = containerClient.GetBlobClient(fileName);
            using var memoryStream = new MemoryStream(bytes);
            blobClient.Upload(memoryStream, overwrite: true);
            var blobUri = blobClient.Uri.ToString();

            user.AvatarLink = blobUri;
            await _userRepository.UpdateUserAsync(user);
            return blobUri;
        }
    }
}
