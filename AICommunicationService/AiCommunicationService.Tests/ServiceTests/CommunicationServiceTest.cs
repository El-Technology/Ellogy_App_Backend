using AICommunicationService.BLL.Constants;
using AICommunicationService.BLL.Hubs;
using AICommunicationService.BLL.Interfaces;
using AICommunicationService.BLL.Services;
using AICommunicationService.Common.Enums;
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
        private ICommunicationService _communicationService;

        [SetUp]
        public void Setup()
        {
            _promptRepositoryMock = new Mock<IAIPromptRepository>();
            _hubContextMock = new Mock<IHubContext<StreamAiHub>>();
            _customAiServiceMock = new Mock<IAzureOpenAiRequestService>();
            _communicationService = new CommunicationService(_promptRepositoryMock.Object, _hubContextMock.Object, _customAiServiceMock.Object);
        }

        [Test]
        public async Task ChatRequestAsync_ShouldCallPostAiRequestAsyncWithCorrectMessageRequest()
        {
            var createConversationRequest = new CreateConversationRequest
            {
                Temperature = 0.9f,
                TemplateName = "TemplateName",
                UserInput = "UserInput",
                AiModelEnum = AiModelEnum.Turbo
            };

            var expectedAiResponse = "ExpectedAiResponse";

            _promptRepositoryMock.Setup(x => x.GetPromptByTemplateNameAsync(It.IsAny<string>()))
                                 .ReturnsAsync(new AIPrompt { Value = "Template Value", Functions = "Functions Value" });

            _customAiServiceMock.Setup(x => x.PostAiRequestAsync(It.IsAny<MessageRequest>()))
                                .ReturnsAsync(expectedAiResponse);

            var result = await _communicationService.ChatRequestAsync(createConversationRequest);

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

            var result = await _communicationService.StreamSignalRConversationAsync(streamRequest);

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
            var createConversationRequest = new CreateConversationRequest
            {
                Temperature = 0.9f,
                TemplateName = "TemplateName",
                UserInput = "UserInput",
                AiModelEnum = AiModelEnum.Turbo
            };

            var expectedAiResponse = "ExpectedAiResponse";

            _promptRepositoryMock.Setup(x => x.GetPromptByTemplateNameAsync(It.IsAny<string>()))
                                 .ReturnsAsync(new AIPrompt { Value = "Template Value", Functions = "Functions Value" });

            _customAiServiceMock.Setup(x => x.PostAiRequestWithFunctionAsync(It.IsAny<MessageRequest>()))
                                .ReturnsAsync(expectedAiResponse);

            var result = await _communicationService.ChatRequestWithFunctionAsync(createConversationRequest);

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

        [Test]
        public async Task ReturnStreamingAsync_ShouldCallPostAiRequestAsStreamAsyncWithCorrectMessageRequest()
        {
            var createConversationRequest = new CreateConversationRequest
            {
                Temperature = 0.9f,
                TemplateName = "TemplateName",
                UserInput = "UserInput",
                AiModelEnum = AiModelEnum.Turbo
            };

            var expectedAiResponse = "ExpectedAiResponse";

            _promptRepositoryMock.Setup(x => x.GetPromptByTemplateNameAsync(It.IsAny<string>()))
                                 .ReturnsAsync(new AIPrompt { Value = "Template Value" });

            _customAiServiceMock.Setup(x => x.PostAiRequestAsStreamAsync(It.IsAny<MessageRequest>(), It.IsAny<Func<string, Task>>()))
                                .Callback<MessageRequest, Func<string, Task>>((request, onDataReceived) => onDataReceived(expectedAiResponse));

            await _communicationService.ReturnStreamingAsync(createConversationRequest, async response =>
            {
                Assert.That(response, Is.EqualTo(expectedAiResponse), "The response should match the expected AI response.");
            });

            _promptRepositoryMock.Verify(x => x.GetPromptByTemplateNameAsync(createConversationRequest.TemplateName), Times.Once,
                "GetPromptByTemplateNameAsync should have been called once with the correct template name.");

            _customAiServiceMock.Verify(x => x.PostAiRequestAsStreamAsync(It.Is<MessageRequest>(
                request => request.Temperature == createConversationRequest.Temperature &&
                           request.Template == "Template Value" &&
                           request.UserInput == createConversationRequest.UserInput &&
                           request.Url.Contains(AzureAiConstants.TurboModel)),
                It.IsAny<Func<string, Task>>()), Times.Once,
                "PostAiRequestAsStreamAsync should have been called once with the correct MessageRequest parameters and onDataReceived function.");
        }
    }
}
