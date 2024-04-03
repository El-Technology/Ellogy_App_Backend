using AutoFixture;
using Azure.Messaging.ServiceBus;
using Moq;
using UserManager.BLL.Services;
using UserManager.Common;
using UserManager.Common.Models.NotificationModels;

namespace UserManager.Tests.ServiceTests;

[TestFixture]
public class NotificationQueueServiceTest
{
    private Mock<ServiceBusClient> _busClientMock;
    private NotificationQueueService _notificationQueueService;
    private Fixture _fixture = new();

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _busClientMock = new Mock<ServiceBusClient>();
        _notificationQueueService = new NotificationQueueService(_busClientMock.Object);
    }

    [Test]
    public async Task SendNotificationAsync_ShouldSendNotificationToQueue()
    {
        // Arrange
        var notificationModel = _fixture.Create<NotificationModel>();

        var busSenderMock = new Mock<ServiceBusSender>();
        _busClientMock.Setup(x => x.CreateSender(NotificationQueueOptions.QueueName)).Returns(busSenderMock.Object);

        // Act
        await _notificationQueueService.SendNotificationAsync(notificationModel);

        // Assert
        busSenderMock.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), CancellationToken.None), Times.Once);
    }
}
