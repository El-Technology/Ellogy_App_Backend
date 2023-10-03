using AICommunicationService.BLL.Constants;
using AICommunicationService.BLL.Hubs;
using AICommunicationService.BLL.Interfaces;
using AICommunicationService.Common.Enums;
using AICommunicationService.Common.Models.AIRequest;
using AICommunicationService.DAL.Interfaces;
using Microsoft.AspNetCore.SignalR;
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
        private readonly IAzureOpenAiRequestService _customAiService;

        public CommunicationService(IAIPromptRepository aIPromptRepository, IHubContext<StreamAiHub> hubContext, IAzureOpenAiRequestService azureOpenAiRequestService)
        {
            _hubContext = hubContext;
            _aIPromptRepository = aIPromptRepository;
            _customAiService = azureOpenAiRequestService;
        }

        private async Task<string> GetTemplateAsync(string promptName)
        {
            var getPrompt = await _aIPromptRepository.GetPromptByTemplateNameAsync(promptName)
                ?? throw new Exception("Prompt was not found");
            return getPrompt.Value;
        }

        private async Task<string?> GetFunctionsAsync(string promptName)
        {
            var getPrompt = await _aIPromptRepository.GetPromptByTemplateNameAsync(promptName)
                ?? throw new Exception("Prompt was not found");
            return getPrompt.Functions;
        }

        private string GetAiModelLink(AiModelEnum aiModelEnum)
        {
            string? deploymentName = aiModelEnum switch
            {
                AiModelEnum.Turbo => AzureAiConstants.TurboModel,
                AiModelEnum.Four => AzureAiConstants.FourModel,
                _ => throw new Exception("Wrong enum"),
            };
            return $"{AzureAiConstants.BaseUrl}{deploymentName}/chat/completions?{AzureAiConstants.ApiVersion}";
        }

        /// <inheritdoc cref="ICommunicationService.ChatRequestAsync(CreateConversationRequest)"/>
        public async Task<string?> ChatRequestAsync(CreateConversationRequest createConversationRequest)
        {
            var request = new ConversationRequestWithFunctions
            {
                Temperature = createConversationRequest.Temperature,
                Template = await GetTemplateAsync(createConversationRequest.TemplateName),
                Url = GetAiModelLink(createConversationRequest.AiModelEnum),
                UserInput = createConversationRequest.UserInput
            };

            return await _customAiService.PostAiRequestAsync(request);
        }

        /// <inheritdoc cref="ICommunicationService.StreamSignalRConversationAsync(StreamRequest)"/>
        public async Task<string> StreamSignalRConversationAsync(StreamRequest streamRequest)
        {
            if (!StreamAiHub.listOfConnections.Any(c => c.Equals(streamRequest.ConnectionId)))
                throw new Exception($"We can`t find connectionId => {streamRequest.ConnectionId}");

            var request = new ConversationRequestWithFunctions
            {
                Temperature = streamRequest.Temperature,
                Template = await GetTemplateAsync(streamRequest.TemplateName),
                Url = GetAiModelLink(streamRequest.AiModelEnum),
                UserInput = streamRequest.UserInput
            };

            var stringBuilder = new StringBuilder();
            await _customAiService.PostAiRequestAsStreamAsync(request, async response =>
            {
                await _hubContext.Clients.Client(streamRequest.ConnectionId).SendAsync(streamRequest.SignalMethodName, response);
                stringBuilder.Append(response);
            });

            return stringBuilder.ToString();
        }

        /// <inheritdoc cref="ICommunicationService.ChatRequestWithFunctionAsync(CreateConversationRequest)"/>
        public async Task<string?> ChatRequestWithFunctionAsync(CreateConversationRequest createConversationRequest)
        {
            var request = new ConversationRequestWithFunctions
            {
                Functions = await GetFunctionsAsync(createConversationRequest.TemplateName) ?? throw new Exception("Functions is null, change method or update the prompt"),
                Temperature = createConversationRequest.Temperature,
                Template = await GetTemplateAsync(createConversationRequest.TemplateName),
                Url = GetAiModelLink(createConversationRequest.AiModelEnum),
                UserInput = createConversationRequest.UserInput
            };

            return await _customAiService.PostAiRequestWithFunctionAsync(request);
        }
    }
}
