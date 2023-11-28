using AICommunicationService.BLL.Dtos;
using AICommunicationService.BLL.Interfaces;
using AICommunicationService.Common.Enums;
using AICommunicationService.Common.Models;
using AICommunicationService.Common.Models.AIRequest;
using Newtonsoft.Json;
using System.Text;

namespace AICommunicationService.BLL.Services
{
    public class AzureOpenAiRequestService : IAzureOpenAiRequestService
    {
        private readonly HttpClient _httpClient;

        public AzureOpenAiRequestService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("AzureAiRequest");
        }

        private List<object> GetMessages(string systemMessage, string userInput)
        {
            return new List<object>
            {
                new
                {
                    role = "system",
                    content = systemMessage
                },
                new
                {
                    role = "user",
                    content = userInput
                }
            };
        }

        private StringContent PostAiRequestGetContent(MessageRequest request, AiRequestType requestType)
        {
            var messages = GetMessages(request.Template, request.UserInput);
            object requestData;
            switch (requestType)
            {
                case AiRequestType.Default:
                    requestData = new
                    {
                        messages,
                        temperature = request.Temperature
                    };
                    break;
                case AiRequestType.Functions:
                    var functions = JsonConvert.DeserializeObject<List<FunctionDefinition>>(request.Functions);
                    requestData = new
                    {
                        messages,
                        functions,
                        function_call = new
                        {
                            name = functions?.FirstOrDefault()?.Name
                        },
                        temperature = request.Temperature
                    };
                    break;
                case AiRequestType.Streaming:
                    requestData = new
                    {
                        messages,
                        temperature = request.Temperature,
                        stream = true
                    };
                    break;
                default:
                    throw new NotImplementedException();
            }
            var jsonRequest = JsonConvert.SerializeObject(requestData);
            return new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        }

        /// <inheritdoc cref="IAzureOpenAiRequestService.PostAiRequestWithFunctionAsync(MessageRequest)"/>
        public async Task<CommunicationResponseModel> PostAiRequestWithFunctionAsync(MessageRequest request)
        {
            var content = PostAiRequestGetContent(request, AiRequestType.Functions);

            var result = await _httpClient.PostAsync(request.Url, content);
            var resultAsObject = JsonConvert.DeserializeObject<AiResponseModel>(await result.Content.ReadAsStringAsync());

            var communicationModel = new CommunicationResponseModel
            {
                Content = resultAsObject?.Choices?.FirstOrDefault()?.Message?.FunctionCall?.Arguments,
                Usage = resultAsObject?.Usage
            };

            return communicationModel;
        }

        /// <inheritdoc cref="IAzureOpenAiRequestService.PostAiRequestAsync(MessageRequest)"/>
        public async Task<CommunicationResponseModel> PostAiRequestAsync(MessageRequest request)
        {
            var content = PostAiRequestGetContent(request, AiRequestType.Default);

            var result = await _httpClient.PostAsync(request.Url, content);
            var resultAsObject = JsonConvert.DeserializeObject<AiResponseModel>(await result.Content.ReadAsStringAsync());

            var communicationModel = new CommunicationResponseModel
            {
                Content = resultAsObject?.Choices?.FirstOrDefault()?.Message?.Content,
                Usage = resultAsObject?.Usage
            };

            return communicationModel;
        }

        /// <inheritdoc cref="IAzureOpenAiRequestService.PostAiRequestAsStreamAsync(MessageRequest, Func{string, Task})"/>
        public async Task PostAiRequestAsStreamAsync(MessageRequest request, Func<string, Task> onDataReceived)
        {
            var content = PostAiRequestGetContent(request, AiRequestType.Streaming);

            var message = new HttpRequestMessage { RequestUri = new Uri(request.Url), Method = HttpMethod.Post, Content = content };
            var response = await _httpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);
            using var stream = await response.Content.ReadAsStreamAsync();
            using (var streamReader = new StreamReader(stream))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = await streamReader.ReadLineAsync();
                    if (string.IsNullOrEmpty(line))
                        continue;

                    var aiResponse = new AiResponseModel();
                    try
                    {
                        aiResponse = JsonConvert.DeserializeObject<AiResponseModel>(line.Replace("data: ", ""));
                    }
                    catch (Exception) { continue; };

                    var stringAiResponse = aiResponse?.Choices?.FirstOrDefault()?.Delta?.Content;
                    if (!string.IsNullOrEmpty(stringAiResponse))
                        await onDataReceived(stringAiResponse);
                }
            }
        }
    }
}
