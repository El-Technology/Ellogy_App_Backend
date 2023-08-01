using AICommunicationService.BLL.Helpers;
using AICommunicationService.BLL.Interfaces;
using AICommunicationService.Common.Models.AIRequest;
using AICommunicationService.Common.Models.AIResponse;
using Newtonsoft.Json;
using OpenAI_API;
using OpenAI_API.Chat;

namespace AICommunicationService.BLL.Services
{
    /// <summary>
    /// This class provides an implementation for communication with Chat GPT using different templates. It utilizes the methods defined in the <see cref="ICommunicationService"/> interface.
    /// </summary>
    public class CommunicationService : ICommunicationService
    {
        private readonly OpenAIAPI _openAIAPI;
        public CommunicationService(OpenAIAPI openAIAPI)
        {
            _openAIAPI = openAIAPI;
        }

        /// <summary>
        /// This private asynchronous method sends a chat request to the OpenAI API, which contains user input, and retrieves the model's response. It returns the generated response as a string.
        /// </summary>
        /// <param name="chatRequest">A model representing the chat request, which includes user input for the conversation.</param>
        /// <returns>A string representing the model's response to the user input in the chat conversation.</returns>
        /// <exception cref="Exception"></exception>
        private async Task<string> GetStringResultAsync(ChatRequest chatRequest)
        {
            var chatResult = await _openAIAPI.Chat.CreateChatCompletionAsync(chatRequest);
            var stringResult = chatResult.Choices.FirstOrDefault()?.Message.Content
                ?? throw new Exception("Get result exception");
            return stringResult;
        }

        /// <summary>
        /// This private asynchronous generic method sends a chat request to the OpenAI API, which contains user input, and retrieves the model's response as a JSON string.
        /// It then deserializes the JSON string into a specified generic type 'T' and returns the deserialized object.
        /// </summary>
        /// <typeparam name="T">The type to which the JSON string should be deserialized.</typeparam>
        /// <param name="chatRequest">A model representing the chat request, which includes user input for the conversation.</param>
        /// <returns>An object of type 'T' representing the deserialized model response from the chat conversation.</returns>
        /// <exception cref="Exception"></exception>
        private async Task<T> GetResultAsync<T>(ChatRequest chatRequest)
        {
            var stringResult = await GetStringResultAsync(chatRequest);

            var gptResponse = JsonConvert.DeserializeObject<T>(stringResult)
                ?? throw new Exception("Deserializing Error");

            return gptResponse;
        }

        /// <inheritdoc cref="ICommunicationService.SendMessageAsync(string)"/>
        public async Task<string> SendMessageAsync(string message)
        {
            var result = await _openAIAPI.Completions.GetCompletion(message);
            return result;
        }

        /// <inheritdoc cref="ICommunicationService.GetDescriptionAsync(string)"/>
        public async Task<DescriptionResponse> GetDescriptionAsync(string userStories)
        {
            var chatRequest = ChatRequestHelper.GetDescriptionRequest(userStories);
            var response = await GetResultAsync<DescriptionResponse>(chatRequest);
            return response;
        }

        /// <inheritdoc cref="ICommunicationService.GetDiagramsAsync(string)"/>
        public async Task<DiagramResponse> GetDiagramsAsync(string userStories)
        {
            var chatRequest = ChatRequestHelper.GetDiagramsRequest(userStories);
            var response = await GetResultAsync<DiagramResponse>(chatRequest);
            return response;
        }

        /// <inheritdoc cref="ICommunicationService.GetIsRequestClearAsync(string)"/>
        public async Task<bool> GetIsRequestClearAsync(string history)
        {
            var chatRequest = ChatRequestHelper.GetIsRequestClear(history);
            var response = await GetStringResultAsync(chatRequest);
            return bool.Parse(response);
        }

        /// <inheritdoc cref="ICommunicationService.GetPotentialSummaryAsync(string)"/>
        public async Task<List<PotentialSummaryResponse>> GetPotentialSummaryAsync(string description)
        {
            var chatRequest = ChatRequestHelper.GetPotentialSummary(description);
            var response = await GetResultAsync<List<PotentialSummaryResponse>>(chatRequest);
            return response;
        }

        /// <inheritdoc cref="ICommunicationService.GetSummaryAsync(string)"/>
        public async Task<List<SummaryResponse>> GetSummaryAsync(string history)
        {
            var chatRequest = ChatRequestHelper.GetSummary(history);
            var response = await GetResultAsync<List<SummaryResponse>>(chatRequest);
            return response;
        }

        /// <inheritdoc cref="ICommunicationService.GetConversationAsync(ConversationRequest)"/>
        public async Task<string> GetConversationAsync(ConversationRequest conversationRequest)
        {
            var chatRequest = ChatRequestHelper.GetConversation(conversationRequest);
            var response = await GetStringResultAsync(chatRequest);
            return response;
        }
    }
}
