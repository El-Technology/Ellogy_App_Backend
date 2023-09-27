using AICommunicationService.BLL.Constants;
using AICommunicationService.BLL.Hubs;
using AICommunicationService.BLL.Interfaces;
using AICommunicationService.Common.Enums;
using AICommunicationService.Common.Models.AIRequest;
using AICommunicationService.DAL.Interfaces;
using Microsoft.AspNetCore.SignalR;
using OpenAI_API;
using OpenAI_API.Chat;
using System.Text;
using TicketsManager.Common;

namespace AICommunicationService.BLL.Services
{
    /// <summary>
    /// This class provides an implementation for communication with Chat GPT using different templates. It utilizes the methods defined in the <see cref="ICommunicationService"/> interface.
    /// </summary>
    public class CommunicationService : ICommunicationService
    {
        private readonly IHubContext<StreamAiHub> _hubContext;
        private readonly IAIPromptRepository _aIPromptRepository;

        public CommunicationService(IAIPromptRepository aIPromptRepository, IHubContext<StreamAiHub> hubContext)
        {
            _hubContext = hubContext;
            _aIPromptRepository = aIPromptRepository;
        }

        private async Task<string> GetTemplateAsync(string promptName)
        {
            var getPrompt = await _aIPromptRepository.GetPromptByTemplateNameAsync(promptName)
                ?? throw new Exception("Prompt was not found");
            return getPrompt.Value;
        }

        private async Task<Conversation> CreateChatConversationAsync(CreateConversationRequest createConversationRequest)
        {
            string? deploymentName = createConversationRequest.AiModelEnum switch
            {
                AiModelEnum.Turbo => AzureAiConstants.TurboModel,
                AiModelEnum.Four => AzureAiConstants.FourModel,
                _ => throw new Exception("Wrong enum"),
            };

            var azureOpenAI = OpenAIAPI.ForAzure(YourResourceName: AzureAiConstants.ResourceName, deploymentId: deploymentName, apiKey: EnvironmentVariables.OpenAiKey);
            azureOpenAI.ApiVersion = AzureAiConstants.ApiVersion;

            var createConversation = azureOpenAI.Chat.CreateConversation();
            var template = await GetTemplateAsync(createConversationRequest.TemplateName);
            createConversation.AppendSystemMessage(template);
            createConversation.AppendUserInput(createConversationRequest.UserInput);
            createConversation.RequestParameters.Temperature = createConversationRequest.Temperature;

            return createConversation;
        }

        /// <inheritdoc cref="ICommunicationService.ChatRequestAsync(CreateConversationRequest)"/>
        public async Task<string> ChatRequestAsync(CreateConversationRequest createConversationRequest)
        {
            return await (await CreateChatConversationAsync(createConversationRequest)).GetResponseFromChatbotAsync();
        }

        /// <inheritdoc cref="ICommunicationService.StreamSignalRConversationAsync(StreamRequest)"/>
        public async Task<string> StreamSignalRConversationAsync(StreamRequest streamRequest)
        {
            if (!StreamAiHub.listOfConnections.Any(c => c.Equals(streamRequest.ConnectionId)))
                throw new Exception($"We can`t find connectionId => {streamRequest.ConnectionId}");

            var createConversation = CreateChatConversationAsync(streamRequest);
            var stringBuilder = new StringBuilder();
            await (await createConversation).StreamResponseFromChatbotAsync(async res =>
            {
                await _hubContext.Clients.Client(streamRequest.ConnectionId).SendAsync(streamRequest.SignalMethodName, res);
                stringBuilder.Append(res);
            });

            return stringBuilder.ToString();
        }
    }
}
