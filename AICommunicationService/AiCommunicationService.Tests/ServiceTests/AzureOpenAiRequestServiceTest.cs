using AICommunicationService.BLL.Services;
using AICommunicationService.Common.Models;
using AICommunicationService.Common.Models.AIRequest;
using Moq;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Moq.Protected;

namespace AiCommunicationService.Tests.ServiceTests
{
    [TestFixture]
    public class AzureOpenAiRequestServiceTest
    {
        private AzureOpenAiRequestService ReturnAiRequestServiceAsync(AiResponseModel aiResponseModel)
        {
            var serializedResponse = JsonConvert.SerializeObject(aiResponseModel);
            var content = new StringContent(serializedResponse, Encoding.UTF8, "application/json");

            var mockHttpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = content });

            var httpClient = new HttpClient(mockHttpMessageHandler);

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>()))
                                .Returns(httpClient);

            return new AzureOpenAiRequestService(mockHttpClientFactory.Object);
        }

        [Test]
        public async Task PostAiRequestWithFunctionAsync_ReturnsGptResponse()
        {
            var expectedArguments = "some arguments";
            var messageRequest = new MessageRequest
            {
                Temperature = 0.9f,
                UserInput = "some user input",
                Template = "some template",
                //Example of function which stored in db
                Functions = "[{'name':'Json','parameters':{'type':'object','required':['title','description'],'properties':{'title':{'type':'string'},'description':{'type':'string'}}}}]",
                Url = "http://example.com/api/ai"
            };

            var aiResponseModel = new AiResponseModel
            {
                Choices = new()
                {
                    new Choice
                    {
                       Message = new Message
                       {
                           FunctionCall = new FunctionCall
                           {
                               Arguments = expectedArguments
                           }
                       }
                    }
                }
            };

            var aiService = ReturnAiRequestServiceAsync(aiResponseModel);

            var result = await aiService.PostAiRequestWithFunctionAsync(messageRequest);

            Assert.That(result, Is.EqualTo(expectedArguments));
        }

        [Test]
        public async Task PostAiRequestAsync_ReturnsGptResponse()
        {
            var expectedArguments = "some arguments";
            var messageRequest = new MessageRequest
            {
                Temperature = 0.9f,
                UserInput = "some user input",
                Template = "some template",
                Url = "http://example.com/api/ai"
            };

            var aiResponseModel = new AiResponseModel
            {
                Choices = new()
                {
                    new Choice
                    {
                       Message = new Message
                       {
                           Content = expectedArguments
                       }
                    }
                }
            };

            var aiService = ReturnAiRequestServiceAsync(aiResponseModel);

            var result = await aiService.PostAiRequestAsync(messageRequest);

            Assert.That(result, Is.EqualTo(expectedArguments));
        }

        [Test]
        public async Task PostAiRequestAsStreamAsync_ShouldCallOnDataReceivedWithCorrectResponse()
        {
            var expectedResponse = "some response";
            var messageRequest = new MessageRequest
            {
                Temperature = 0.9f,
                UserInput = "some user input",
                Template = "some template",
                Url = "http://example.com/api/ai"
            };

            var aiResponseModel = new AiResponseModel
            {
                Choices = new List<Choice>
                {
                    new Choice
                    {
                        Delta = new Delta
                        {
                            Content = expectedResponse
                        }
                    }
                }
            };

            var aiService = ReturnAiRequestServiceAsync(aiResponseModel);

            var onDataReceivedCalled = false;
            async Task OnDataReceived(string data)
            {
                onDataReceivedCalled = true;
                Assert.That(data, Is.EqualTo(expectedResponse));
            }

            await aiService.PostAiRequestAsStreamAsync(messageRequest, OnDataReceived);

            Assert.That(onDataReceivedCalled, Is.True, "onDataReceived should have been called with the correct response.");
        }
    }
}
