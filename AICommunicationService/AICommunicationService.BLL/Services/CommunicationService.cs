using AICommunicationService.BLL.Hubs;
using AICommunicationService.BLL.Interfaces;
using AICommunicationService.Common.Models.AIRequest;
using AICommunicationService.DAL.Interfaces;
using Microsoft.AspNetCore.SignalR;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System.Text;

namespace AICommunicationService.BLL.Services
{
    /// <summary>
    /// This class provides an implementation for communication with Chat GPT using different templates. It utilizes the methods defined in the <see cref="ICommunicationService"/> interface.
    /// </summary>
    public class CommunicationService : ICommunicationService
    {
        private readonly IHubContext<StreamAiHub> _hubContext;
        private readonly IAIPromptRepository _aIPromptRepository;
        private readonly OpenAIAPI _openAIAPI;
        public CommunicationService(OpenAIAPI openAIAPI, IAIPromptRepository aIPromptRepository, IHubContext<StreamAiHub> hubContext)
        {
            _hubContext = hubContext;
            _aIPromptRepository = aIPromptRepository;
            _openAIAPI = openAIAPI;
        }

        private async Task<string> GetTemplateAsync(string promptName)
        {
            var getPrompt = await _aIPromptRepository.GetPromptByTemplateNameAsync(promptName)
                ?? throw new Exception("Prompt was not found");
            return getPrompt.Value;
        }

        private async Task<Conversation> CreateChatConversationAsync(CreateConversationRequest createConversationRequest, Model gptModel)
        {
            var createConversation = _openAIAPI.Chat.CreateConversation();
            var template = await GetTemplateAsync(createConversationRequest.TemplateName);
            createConversation.AppendSystemMessage(template);
            createConversation.AppendUserInput(createConversationRequest.UserInput);
            createConversation.Model = gptModel;
            createConversation.RequestParameters.Temperature = createConversationRequest.Temperature;

            return createConversation;
        }

        public async Task<string> CreateChatCompletionAsync(CreateConversationRequest createConversationRequest)
        {
            var template = await GetTemplateAsync(createConversationRequest.TemplateName);
            var chatCompletion = await _openAIAPI.Chat.CreateChatCompletionAsync(new ChatRequest
            {
                Model = Model.ChatGPTTurbo,
                Temperature = createConversationRequest.Temperature,
                Messages = new ChatMessage[]
                {
                    new ChatMessage(ChatMessageRole.User, template + createConversationRequest.UserInput)
                }
            });
            var stringResult = chatCompletion.Choices.FirstOrDefault()?.Message.Content
                ?? throw new Exception("Taking result error, try again");

            return stringResult;
        }

        /// <inheritdoc cref="ICommunicationService.ChatRequestAsync(CreateConversationRequest)"/>
        public async Task<string> ChatRequestAsync(CreateConversationRequest createConversationRequest)
        {
            return await (await CreateChatConversationAsync(createConversationRequest, Model.ChatGPTTurbo)).GetResponseFromChatbotAsync();
        }

        /// <inheritdoc cref="ICommunicationService.StreamSignalRConversationAsync(StreamRequest)"/>
        public async Task<string> StreamSignalRConversationAsync(StreamRequest streamRequest)
        {
            if (!StreamAiHub.listOfConnections.Any(c => c.Equals(streamRequest.ConnectionId)))
                throw new Exception($"We can`t find connectionId => {streamRequest.ConnectionId}");

            var createConversation = CreateChatConversationAsync(streamRequest, Model.ChatGPTTurbo);
            var stringBuilder = new StringBuilder();
            await (await createConversation).StreamResponseFromChatbotAsync(async res =>
            {
                await _hubContext.Clients.Client(streamRequest.ConnectionId).SendAsync(streamRequest.SignalMethodName, res);
                stringBuilder.Append(res);
            });

            return stringBuilder.ToString();
        }

        public async Task<string> ChatRequestGptFourAsync(CreateConversationRequest createConversationRequest)
        {
            return await (await CreateChatConversationAsync(createConversationRequest, Model.GPT4)).GetResponseFromChatbotAsync();
        }
    }
}
