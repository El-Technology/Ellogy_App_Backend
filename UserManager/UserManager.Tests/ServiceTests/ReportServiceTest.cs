using AutoFixture;
using Azure;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Moq;
using System.Text;
using UserManager.BLL.Dtos;
using UserManager.BLL.Dtos.RegisterDtos;
using UserManager.BLL.Services;
using UserManager.DAL.Repositories;

namespace UserManager.Tests.ServiceTests
{
    [TestFixture]
    public class ReportServiceTest : BaseClassForServices
    {

        [Test]
        public async Task SendReportAsync_SendMessageToEmail()
        {
            var userRepository = new UserRepository(_userManagerDbContext);
            var registerService = new RegisterService(_mapper, userRepository);
            var serviceBusClientMock = new Mock<ServiceBusClient>();
            var serviceBusSenderMock = new Mock<ServiceBusSender>();
            var blobServiceClientMock = new Mock<BlobServiceClient>();
            var containerClientMock = new Mock<BlobContainerClient>();
            var blobContentInfoMock = new Mock<BlobContentInfo>();
            var blobClientMock = new Mock<BlobClient>();
            var responseMock = new Mock<Response>();

            //Send message service
            serviceBusClientMock.Setup(x => x.CreateSender(It.IsAny<string>())).Returns(serviceBusSenderMock.Object);
            var notificationQueueService = new NotificationQueueService(serviceBusClientMock.Object);

            //Service for uploading files to storage
            blobClientMock.Setup(x => x.UploadAsync(It.IsAny<Stream>(), true, default))
                .ReturnsAsync(Response.FromValue(blobContentInfoMock.Object, responseMock.Object));
            containerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);
            blobServiceClientMock.Setup(client => client.GetBlobContainerClient(It.IsAny<string>())).Returns(containerClientMock.Object);

            //Report service
            var reportService = new ReportService(userRepository, notificationQueueService, blobServiceClientMock.Object);

            //Creating new user
            var user = _fixture.Create<UserRegisterRequestDto>();
            await registerService.RegisterUserAsync(user);

            //Creating report model
            var bytesForBase64 = Encoding.UTF8.GetBytes(_fixture.Create<string>());
            var base64String = Convert.ToBase64String(bytesForBase64);
            var reportModel = new ReportModel
            {
                Base64JpgFiles = new List<string> { base64String },
                Category = "someCategory",
                Option = "someOption",
                ReceiverEmail = _fixture.Create<string>(),
                UserEmail = user.Email,
                UserText = "someUserInput"
            };

            //Send report
            await reportService.SendReportAsync(reportModel);

            // Check if message was sent
            serviceBusSenderMock.Verify(sender => sender.SendMessageAsync(It.IsAny<ServiceBusMessage>(), CancellationToken.None), Times.Once);

            // Check upload to Blob Storage
            blobClientMock.Verify(x => x.UploadAsync(It.IsAny<Stream>(), true, default), Times.Once);
        }
    }
}
