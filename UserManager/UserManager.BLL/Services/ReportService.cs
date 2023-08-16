using Azure.Storage.Blobs;
using UserManager.BLL.Dtos;
using UserManager.BLL.Exceptions;
using UserManager.BLL.Interfaces;
using UserManager.Common.Constants;
using UserManager.Common.Models.NotificationModels;
using UserManager.DAL.Interfaces;

namespace UserManager.BLL.Services
{
    public class ReportService : IReportService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IUserRepository _userRepository;
        private readonly INotificationQueueService _notificationQueueService;
        private readonly NotificationModel _notificationModel = new()
        {
            BlobUrls = new(),
            Type = NotificationTypeEnum.Report,
            Way = NotificationWayEnum.Email
        };
        private const string UserNamePattern = "{{{userName}}}";
        private const string UserEmailPattern = "{{{userEmail}}}";
        private const string UserTextPattern = "{{{userText}}}";
        public ReportService(IUserRepository userRepository, INotificationQueueService notificationQueueService, BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
            _userRepository = userRepository;
            _notificationQueueService = notificationQueueService;
        }

        /// <inheritdoc cref="IReportService.SendReportAsync(ReportModel)" />
        public async Task SendReportAsync(ReportModel reportModel)
        {
            if (!await _userRepository.CheckEmailIsExistAsync(reportModel.UserEmail))
                throw new UserNotFoundException(reportModel.UserEmail);

            var user = (await _userRepository.GetUserByEmailAsync(reportModel.UserEmail))!;

            var containerClient = _blobServiceClient.GetBlobContainerClient(BlobContainerConstants.ImagesContainer);


            for (int i = 0; i < reportModel.Base64JpgFiles.Count; i++)
            {
                string? file = reportModel.Base64JpgFiles[i];
                var bytes = Convert.FromBase64String(file);
                var fileName = $"scr{i}.jpg";
                var blobClient = containerClient.GetBlobClient(fileName);
                using (var memoryStream = new MemoryStream(bytes))
                {
                    blobClient.Upload(memoryStream, overwrite: true);
                }

                _notificationModel.BlobUrls.Add(fileName);
            }

            _notificationModel.Consumer = reportModel.ReceiverEmail;
            _notificationModel.MetaData = new()
            {
                { UserNamePattern, $"{user.FirstName} {user.LastName}" },
                { UserEmailPattern, user.Email },
                { UserTextPattern, reportModel.UserText }
            };

            await _notificationQueueService.SendNotificationAsync(_notificationModel);
        }
    }
}
