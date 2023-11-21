using AICommunicationService.BLL.Constants;
using AICommunicationService.BLL.Dtos;
using AICommunicationService.BLL.Hubs;
using AICommunicationService.BLL.Interfaces;
using AICommunicationService.BLL.Services;
using AICommunicationService.Common.Enums;
using AICommunicationService.Common.Models;
using AICommunicationService.Common.Models.AIRequest;
using AICommunicationService.DAL.Interfaces;
using AICommunicationService.DAL.Models;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace AiCommunicationService.Tests.ServiceTests
{
    [TestFixture]
    public class CommunicationServiceTests
    {
        private Mock<IAIPromptRepository> _promptRepositoryMock;
        private Mock<IHubContext<StreamAiHub>> _hubContextMock;
        private Mock<IAzureOpenAiRequestService> _customAiServiceMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IWalletRepository> _walletRepositoryMock;
        private ICommunicationService _communicationService;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();

            var user = new User
            {
                Id = Guid.NewGuid(),
                TotalPurchasedPoints = 1000,
                TotalPointsUsage = 1000
            };
            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);

            _walletRepositoryMock = new Mock<IWalletRepository>();
            _promptRepositoryMock = new Mock<IAIPromptRepository>();
            _hubContextMock = new Mock<IHubContext<StreamAiHub>>();
            _customAiServiceMock = new Mock<IAzureOpenAiRequestService>();
            _communicationService = new CommunicationService(_promptRepositoryMock.Object,
                                                             _hubContextMock.Object,
                                                             _customAiServiceMock.Object,
                                                             _userRepositoryMock.Object,
                                                             _walletRepositoryMock.Object);
        }

        [Test]
        public async Task ChatRequestAsync_ShouldCallPostAiRequestAsyncWithCorrectMessageRequest()
        {
            var userId = Guid.NewGuid();
            var createConversationRequest = new CreateConversationRequest
            {
                Temperature = 0.9f,
                TemplateName = "TemplateName",
                UserInput = "UserInput",
                AiModelEnum = AiModelEnum.Turbo
            };

            var expectedAiResponse = "ExpectedAiResponse";
            var usage = new Usage
            {
                CompletionTokens = 2,
                PromptTokens = 5,
                TotalTokens = 7
            };
            var communicationResponseModel = new CommunicationResponseModel
            {
                Content = expectedAiResponse,
                Usage = usage
            };

            _promptRepositoryMock.Setup(x => x.GetPromptByTemplateNameAsync(It.IsAny<string>()))
                                 .ReturnsAsync(new AIPrompt { Value = "Template Value", Functions = "Functions Value" });

            _customAiServiceMock.Setup(x => x.PostAiRequestAsync(It.IsAny<MessageRequest>()))
                                .ReturnsAsync(communicationResponseModel);

            var result = await _communicationService.ChatRequestAsync(userId, createConversationRequest);

            _promptRepositoryMock.Verify(x => x.GetPromptByTemplateNameAsync(createConversationRequest.TemplateName), Times.Once,
                "GetPromptByTemplateNameAsync should have been called once with the correct template name.");

            _customAiServiceMock.Verify(x => x.PostAiRequestAsync(It.Is<MessageRequest>(
                request => request.Temperature == createConversationRequest.Temperature &&
                           request.Template == "Template Value" &&
                           request.UserInput == createConversationRequest.UserInput &&
                           request.Url.Contains(AzureAiConstants.TurboModel))),
                           Times.Once,
                           "PostAiRequestAsync should have been called once with the correct MessageRequest parameters.");

            Assert.That(result, Is.EqualTo(expectedAiResponse), "The method should return the expected AI response.");
        }

        [Test]
        public async Task StreamSignalRConversationAsync_ShouldCallPostAiRequestAsStreamAsyncWithCorrectMessageRequest()
        {
            var userId = Guid.NewGuid();
            var streamRequest = new StreamRequest
            {
                Temperature = 0.9f,
                TemplateName = "TemplateName",
                UserInput = "UserInput",
                AiModelEnum = AiModelEnum.Turbo,
                ConnectionId = "ConnectionId",
                SignalMethodName = "SignalMethodName"
            };

            StreamAiHub.listOfConnections.Add(streamRequest.ConnectionId);

            _hubContextMock.Setup(x => x.Clients.Client(streamRequest.ConnectionId).SendCoreAsync(streamRequest.SignalMethodName, It.IsAny<object[]>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            var expectedAiResponse = "ExpectedAiResponse";

            _promptRepositoryMock.Setup(x => x.GetPromptByTemplateNameAsync(It.IsAny<string>()))
                                 .ReturnsAsync(new AIPrompt { Value = "Template Value" });

            _customAiServiceMock.Setup(x => x.PostAiRequestAsStreamAsync(It.IsAny<MessageRequest>(), It.IsAny<Func<string, Task>>()))
                                .Returns(async (MessageRequest request, Func<string, Task> onDataReceived) =>
                                {
                                    await onDataReceived(expectedAiResponse);
                                });

            var result = await _communicationService.StreamSignalRConversationAsync(userId, streamRequest);

            _promptRepositoryMock.Verify(x => x.GetPromptByTemplateNameAsync(streamRequest.TemplateName), Times.Once,
                "GetPromptByTemplateNameAsync should have been called once with the correct template name.");

            _customAiServiceMock.Verify(x => x.PostAiRequestAsStreamAsync(It.Is<MessageRequest>(
                request => request.Temperature == streamRequest.Temperature &&
                           request.Template == "Template Value" &&
                           request.UserInput == streamRequest.UserInput &&
                           request.Url.Contains(AzureAiConstants.TurboModel)),
                It.IsAny<Func<string, Task>>()), Times.Once,
                "PostAiRequestAsStreamAsync should have been called once with the correct MessageRequest parameters and onDataReceived function.");

            Assert.That(result, Is.EqualTo(expectedAiResponse), "The method should return the expected AI response.");
        }

        [Test]
        public async Task ChatRequestWithFunctionAsync_ShouldCallPostAiRequestWithFunctionAsyncWithCorrectMessageRequest()
        {
            var userId = Guid.NewGuid();
            var createConversationRequest = new CreateConversationRequest
            {
                Temperature = 0.9f,
                TemplateName = "TemplateName",
                UserInput = "UserInput",
                AiModelEnum = AiModelEnum.Turbo
            };

            var expectedAiResponse = "ExpectedAiResponse";
            var usage = new Usage
            {
                CompletionTokens = 2,
                PromptTokens = 5,
                TotalTokens = 7
            };
            var communicationResponseModel = new CommunicationResponseModel
            {
                Content = expectedAiResponse,
                Usage = usage
            };
            _promptRepositoryMock.Setup(x => x.GetPromptByTemplateNameAsync(It.IsAny<string>()))
                                 .ReturnsAsync(new AIPrompt { Value = "Template Value", Functions = "Functions Value" });

            _customAiServiceMock.Setup(x => x.PostAiRequestWithFunctionAsync(It.IsAny<MessageRequest>()))
                                .ReturnsAsync(communicationResponseModel);

            var result = await _communicationService.ChatRequestWithFunctionAsync(userId, createConversationRequest);

            _promptRepositoryMock.Verify(x => x.GetPromptByTemplateNameAsync(createConversationRequest.TemplateName), Times.Exactly(2),
                "GetPromptByTemplateNameAsync should have been called once with the correct template name.");

            _customAiServiceMock.Verify(x => x.PostAiRequestWithFunctionAsync(It.Is<MessageRequest>(
                request => request.Temperature == createConversationRequest.Temperature &&
                           request.Template == "Template Value" &&
                           request.UserInput == createConversationRequest.UserInput &&
                           request.Url.Contains(AzureAiConstants.TurboModel))),
                Times.Once, "PostAiRequestWithFunctionAsync should have been called once with the correct MessageRequest parameters.");

            Assert.That(result, Is.EqualTo(expectedAiResponse), "The method should return the expected AI response.");
        }
    }
}
