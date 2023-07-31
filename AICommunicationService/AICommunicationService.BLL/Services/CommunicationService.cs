using AICommunicationService.BLL.Helpers;
using AICommunicationService.BLL.Interfaces;
using AICommunicationService.Common.Models;
using Newtonsoft.Json;
using OpenAI_API;
using OpenAI_API.Chat;

namespace AICommunicationService.BLL.Services
{
    public class CommunicationService : ICommunicationService
    {
        private readonly OpenAIAPI _openAIAPI;
        public CommunicationService(OpenAIAPI openAIAPI)
        {
            _openAIAPI = openAIAPI;
        }

        private async Task<string> GetStringResultAsync(ChatRequest chatRequest)
        {
            var chatResult = await _openAIAPI.Chat.CreateChatCompletionAsync(chatRequest);
            var stringResult = chatResult.Choices.First()?.Message.Content
                ?? throw new Exception("Get result exception");
            return stringResult;
        }

        private async Task<T> GetResultAsync<T>(ChatRequest chatRequest)
        {
            var stringResult = await GetStringResultAsync(chatRequest);

            var gptResponse = JsonConvert.DeserializeObject<T>(stringResult)
                ?? throw new Exception("Deserializing Error");

            return gptResponse;
        }

        public async Task<string> SendMessageAsync(string message)
        {
            var result = await _openAIAPI.Completions.GetCompletion(message);
            return result;
        }

        public async Task<DescriptionResponse> GetDescriptionAsync(string userStories)
        {
            var chatRequest = ChatRequestHelper.GetDescriptionRequest(userStories);
            var response = await GetResultAsync<DescriptionResponse>(chatRequest);
            return response;
        }

        public async Task<DiagramResponse> GetDiagramsAsync(string userStories)
        {
            var chatRequest = ChatRequestHelper.GetDiagramsRequest(userStories);
            var response = await GetResultAsync<DiagramResponse>(chatRequest);
            return response;
        }

        public async Task<bool> GetIsRequestClearAsync(string history)
        {
            var chatRequest = ChatRequestHelper.GetIsRequestClear(history);
            var response = await GetStringResultAsync(chatRequest);
            return bool.Parse(response);
        }

        public async Task<List<PotentialSummaryResponse>> GetPotentialSummaryAsync(string description)
        {
            var chatRequest = ChatRequestHelper.GetPotentialSummary(description);
            var response = await GetResultAsync<List<PotentialSummaryResponse>>(chatRequest);
            return response;
        }
    }
}
