using AICommunicationService.BLL.Constants;
using AICommunicationService.BLL.Exceptions;
using AICommunicationService.BLL.Helpers;
using AICommunicationService.BLL.Interfaces;
using AICommunicationService.Common.Models.AIRequest;
using AICommunicationService.Common.Models.AIResponse;
using AICommunicationService.DAL.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAI_API;
using OpenAI_API.Chat;
using System.Diagnostics;

namespace AICommunicationService.BLL.Services
{
    /// <summary>
    /// This class provides an implementation for communication with Chat GPT using different templates. It utilizes the methods defined in the <see cref="ICommunicationService"/> interface.
    /// </summary>
    public class CommunicationService : ICommunicationService
    {
        private readonly IAIPromptRepository _aIPromptRepository;
        private const bool Stable = true;
        private const bool Random = false;
        private readonly OpenAIAPI _openAIAPI;
        public CommunicationService(OpenAIAPI openAIAPI, IAIPromptRepository aIPromptRepository)
        {
            _aIPromptRepository = aIPromptRepository;
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
            var jsonSetting = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            T? gptResponse;
            try
            {
                gptResponse = JsonConvert.DeserializeObject<T>(stringResult, jsonSetting);
            }
            catch (Exception)
            {
                throw new DeserializeError(stringResult);
            }
            return gptResponse;
        }

        private async Task<string> GetTemplate(string promptName, string aiTemplateInput)
        {
            var getPrompt = await _aIPromptRepository.GetPromptByTemplateNameAsync(promptName)
                ?? throw new Exception("Prompt was not found");
            return getPrompt.Value + aiTemplateInput;
        }

        /// <inheritdoc cref="ICommunicationService.GetDescriptionAsync(string)"/>
        public async Task<DescriptionResponse> GetDescriptionAsync(string userStories)
        {
            var template = await GetTemplate(nameof(AITemplates.DescriptionTemplate), AITemplates.DescriptionTemplate);
            var chatRequest = ChatRequestHelper.GetChatRequestWithOneInputValue(userStories, Stable, template);
            var response = await GetResultAsync<DescriptionResponse>(chatRequest);
            return response;
        }

        /// <inheritdoc cref="ICommunicationService.GetDiagramsAsync(string)"/>
        public async Task<DiagramResponse> GetDiagramsAsync(string userStories)
        {
            var template = await GetTemplate(nameof(AITemplates.DiagramTemplate), AITemplates.DiagramTemplate);
            var chatRequest = ChatRequestHelper.GetChatRequestWithOneInputValue(userStories, Random, template);
            var response = await GetResultAsync<DiagramResponse>(chatRequest);
            return response;
        }

        /// <inheritdoc cref="ICommunicationService.GetIsRequestClearAsync(string)"/>
        public async Task<bool> GetIsRequestClearAsync(string history)
        {
            var template = await GetTemplate(nameof(AITemplates.IsRequestClearTemplate), AITemplates.IsRequestClearTemplate);
            var chatRequest = ChatRequestHelper.GetChatRequestWithOneInputValue(history, Stable, template);
            var response = await GetStringResultAsync(chatRequest);
            _ = bool.TryParse(response, out bool result);
            return result;
        }

        /// <inheritdoc cref="ICommunicationService.GetPotentialSummaryAsync(string)"/>
        public async Task<List<PotentialSummaryResponse>> GetPotentialSummaryAsync(string description)
        {
            var template = await GetTemplate(nameof(AITemplates.PotentialSummaryTemplate), AITemplates.PotentialSummaryTemplate);
            var chatRequest = ChatRequestHelper.GetChatRequestWithOneInputValue(description, Stable, template);
            var response = await GetResultAsync<List<PotentialSummaryResponse>>(chatRequest);
            return response;
        }

        /// <inheritdoc cref="ICommunicationService.GetSummaryAsync(string)"/>
        public async Task<List<SummaryResponse>> GetSummaryAsync(string history)
        {
            var template = await GetTemplate(nameof(AITemplates.SummaryTemplate), AITemplates.SummaryTemplate);
            var chatRequest = ChatRequestHelper.GetChatRequestWithOneInputValue(history, Stable, template);
            var response = await GetResultAsync<List<SummaryResponse>>(chatRequest);
            return response;
        }

        /// <inheritdoc cref="ICommunicationService.GetConversationAsync(ConversationRequest)"/>
        public async Task<string> GetConversationAsync(ConversationRequest conversationRequest)
        {
            var template = await GetTemplate(nameof(AITemplates.ConversationTemplate), AITemplates.ConversationTemplate);
            var chatRequest = ChatRequestHelper.GetConversationRequest(conversationRequest, template);
            var response = await GetStringResultAsync(chatRequest);
            return response;
        }

        /// <inheritdoc cref="ICommunicationService.GetConversationSummaryAsync(ConversationSummaryRequest)"/>
        public async Task<string> GetConversationSummaryAsync(ConversationSummaryRequest conversationSummaryRequest)
        {
            var template = await GetTemplate(nameof(AITemplates.ConversationSummaryTemplate), AITemplates.ConversationSummaryTemplate);
            var chatRequest = ChatRequestHelper.GetConversationSummaryRequest(conversationSummaryRequest, template);
            var response = await GetStringResultAsync(chatRequest);
            return response;
        }
    }
}
