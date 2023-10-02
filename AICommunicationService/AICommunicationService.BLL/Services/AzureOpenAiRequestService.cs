using AICommunicationService.Common.Models;
using AICommunicationService.Common.Models.AIRequest;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using TicketsManager.Common;

namespace AICommunicationService.BLL.Services
{
    public class AzureOpenAiRequestService
    {
        private readonly string? _apiKey = EnvironmentVariables.OpenAiKey;
        private readonly HttpClient _httpClient = new();

        public AzureOpenAiRequestService()
        {
            _httpClient.DefaultRequestHeaders.Add("api-key", _apiKey);
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

        public async Task<string?> PostAiRequestWithFunctionAsync(ConversationRequestWithFunctions request)
        {
            var functions = JsonConvert.DeserializeObject<List<FunctionDefinition>>(request.Functions);
            var messages = GetMessages(request.Template, request.UserInput);

            var requestData = new
            {
                messages,
                functions,
                function_call = new
                {
                    name = functions?.FirstOrDefault()?.Name
                },
                temperature = request.Temperature
            };

            var jsonRequest = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var result = await _httpClient.PostAsync(request.Url, content);
            var resultAsObject = JsonConvert.DeserializeObject<AiResponseModel>(await result.Content.ReadAsStringAsync());
            return resultAsObject?.Choices.FirstOrDefault()?.Message.FunctionCall?.Arguments;
        }

        private async Task<StringContent> PostAiRequestContentOperationsAsync(ConversationRequestWithFunctions request)
        {
            var messages = GetMessages(request.Template, request.UserInput);

            var requestData = new
            {
                messages,
                temperature = request.Temperature
            };

            var jsonRequest = JsonConvert.SerializeObject(requestData);
            return new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        }

        public async Task<string?> PostAiRequestAsync(ConversationRequestWithFunctions request)
        {
            var content = await PostAiRequestContentOperationsAsync(request);

            var result = await _httpClient.PostAsync(request.Url, content);
            var resultAsObject = JsonConvert.DeserializeObject<AiResponseModel>(await result.Content.ReadAsStringAsync());
            return resultAsObject?.Choices.FirstOrDefault()?.Message.Content;
        }

        public async Task<string?> PostAiRequestAsStreamAsync(ConversationRequestWithFunctions request)
        {
            var messages = GetMessages(request.Template, request.UserInput);

            var requestData = new
            {
                messages,
                temperature = request.Temperature,
                stream = true
            };

            var jsonRequest = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var message = new HttpRequestMessage { RequestUri = new Uri(request.Url), Method = HttpMethod.Post, Content = content };
            var response = await _httpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);
            var stringBuilder = new StringBuilder();
            using (var stream = await response.Content.ReadAsStreamAsync())
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

                    Console.Write(stringAiResponse);
                    stringBuilder.Append(stringAiResponse);
                }
            }
            return stringBuilder.ToString();
        }
    }
}
