using Azure.Storage.Blobs;
using UserManager.BLL.Dtos.ReportDto;
using UserManager.BLL.Exceptions;
using UserManager.BLL.Interfaces;
using UserManager.Common.Constants;
using UserManager.Common.Models.NotificationModels;
using UserManager.DAL.Interfaces;

namespace UserManager.BLL.Services;

/// <summary>
///     Service for report operations
/// </summary>
public class ReportService : IReportService
{
    private const string UserNamePattern = "{{{userName}}}";
    private const string UserEmailPattern = "{{{userEmail}}}";
    private const string UserTextPattern = "{{{userText}}}";
    private const string CategoryPattern = "{{{category}}}";
    private const string OptionPattern = "{{{option}}}";
    private readonly BlobServiceClient _blobServiceClient;

    private readonly NotificationModel _notificationModel = new()
    {
        BlobUrls = new List<string>(),
        Type = NotificationTypeEnum.Report,
        Way = NotificationWayEnum.Email
    };

    private readonly IUserRepository _userRepository;
    private readonly IServiceBusQueue _notificationQueueService;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="userRepository"></param>
    /// <param name="notificationQueueService"></param>
    /// <param name="blobServiceClient"></param>
    public ReportService(IUserRepository userRepository,
        BlobServiceClient blobServiceClient,
        IServiceBusQueue notificationQueueService)
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


        foreach (var file in reportModel.Base64JpgFiles)
        {
            var bytes = Convert.FromBase64String(file);
            var fileName = $"scr{Guid.NewGuid()}.jpg";
            var blobClient = containerClient.GetBlobClient(fileName);
            using (var memoryStream = new MemoryStream(bytes))
            {
                await blobClient.UploadAsync(memoryStream, true);
            }

            _notificationModel.BlobUrls?.Add(fileName);
        }

        _notificationModel.Consumer = reportModel.ReceiverEmail;
        _notificationModel.MetaData = new Dictionary<string, string>
        {
            { CategoryPattern, reportModel.Category },
            { OptionPattern, reportModel.Option },
            { UserNamePattern, $"{user.FirstName} {user.LastName}" },
            { UserEmailPattern, user.Email },
            { UserTextPattern, reportModel.UserText }
        };

        await _notificationQueueService.SendMessageAsync(_notificationModel);
    }
}