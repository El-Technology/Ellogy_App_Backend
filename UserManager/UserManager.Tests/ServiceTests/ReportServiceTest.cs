using AutoFixture;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Moq;
using UserManager.BLL.Dtos.ReportDto;
using UserManager.BLL.Interfaces;
using UserManager.BLL.Services;
using UserManager.Common.Constants;
using UserManager.Common.Models.NotificationModels;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.Tests.ServiceTests;

[TestFixture]
public class ReportServiceTest
{
    private Mock<BlobServiceClient> _blobServiceClient;
    private Mock<IServiceBusQueue> _notificationQueueService;
    private Mock<IUserRepository> _userRepository;
    private ReportService _reportService;
    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _blobServiceClient = new Mock<BlobServiceClient>();
        _notificationQueueService = new Mock<IServiceBusQueue>();
        _userRepository = new Mock<IUserRepository>();
        _reportService = new ReportService(
            _userRepository.Object, _blobServiceClient.Object, _notificationQueueService.Object);
    }

    private static byte[] GenerateRandomImage()
    {
        var random = new Random();
        var buffer = new byte[1024];
        random.NextBytes(buffer);

        return buffer;
    }

    [Test]
    public async Task SendReportAsync_ShouldSendReportToQueue()
    {
        // Arrange
        var reportModel = _fixture.Create<ReportModel>();
        reportModel.Base64JpgFiles = new List<string>
        {
            Convert.ToBase64String(GenerateRandomImage()),
            Convert.ToBase64String(GenerateRandomImage())
        };
        var user = _fixture.Create<User>();
        var _notificationModel = _fixture.Create<NotificationModel>();
        var blobContentInfo = BlobsModelFactory.BlobContentInfo(default, default, default, default, default);

        var containerClientMock = new Mock<BlobContainerClient>();
        var blobClientMock = new Mock<BlobClient>();
        var memoryStreamMock = new Mock<MemoryStream>();

        _userRepository.Setup(x => x.CheckEmailIsExistAsync(reportModel.UserEmail))
            .ReturnsAsync(true);
        _userRepository.Setup(x => x.GetUserByEmailAsync(reportModel.UserEmail))
            .ReturnsAsync(user);
        _blobServiceClient.Setup(x => x.GetBlobContainerClient(BlobContainerConstants.ImagesContainer))
            .Returns(containerClientMock.Object);
        containerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(blobClientMock.Object);
        blobClientMock.Setup(x => x.UploadAsync(memoryStreamMock.Object, true, CancellationToken.None))
            .ReturnsAsync(Response.FromValue(blobContentInfo, default!));

        // Act
        await _reportService.SendReportAsync(reportModel);

        // Assert
        _userRepository.Verify(x => x.CheckEmailIsExistAsync(reportModel.UserEmail), Times.Once);
        _userRepository.Verify(x => x.GetUserByEmailAsync(reportModel.UserEmail), Times.Once);
        _blobServiceClient.Verify(x => x.GetBlobContainerClient(BlobContainerConstants.ImagesContainer), Times.Once);
        containerClientMock.Verify(x => x.GetBlobClient(It.IsAny<string>()), Times.Exactly(reportModel.Base64JpgFiles.Count));
        blobClientMock.Verify(x => x.UploadAsync(It.IsAny<Stream>(), true, It.IsAny<CancellationToken>()),
            Times.Exactly(reportModel.Base64JpgFiles.Count));
    }
}
