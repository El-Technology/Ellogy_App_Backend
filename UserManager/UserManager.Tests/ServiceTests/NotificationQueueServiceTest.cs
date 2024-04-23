//using Azure.Messaging.ServiceBus;
//using Moq;
//using UserManager.BLL.Services;
//using UserManager.Common.Models.NotificationModels;

//namespace UserManager.Tests.ServiceTests
//{
//    [TestFixture]
//    public class NotificationQueueServiceTest
//    {
//        [Test]
//        public async Task SendNotificationAsync_SendsMessageToServiceBus()
//        {
//            var serviceBusClientMock = new Mock<ServiceBusClient>();
//            var serviceBusSenderMock = new Mock<ServiceBusSender>();

//            //Bus configuration
//            serviceBusClientMock.Setup(x => x.CreateSender(It.IsAny<string>())).Returns(serviceBusSenderMock.Object);

//            var notificationQueueService = new NotificationQueueService(serviceBusClientMock.Object);

//            //Try to send notification
//            var notificationModel = new NotificationModel
//            {
//                BlobUrls = new List<string> { "someUrl" },
//                Consumer = "someConsumer",
//                MetaData = new Dictionary<string, string> { { "PatternForReplace", "Value" } },
//                Type = NotificationTypeEnum.ResetPassword,
//                Way = NotificationWayEnum.Email
//            };
//            await notificationQueueService.SendNotificationAsync(notificationModel);

//            serviceBusSenderMock.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), CancellationToken.None), Times.Once);
//        }
//    }
//}
