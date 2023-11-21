using AICommunicationService.BLL.Constants;
using AICommunicationService.BLL.Dtos;
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
        private readonly IWalletRepository _walletRepository;
        private readonly IUserRepository _userRepository;

        public CommunicationService(IAIPromptRepository aIPromptRepository,
                                    IHubContext<StreamAiHub> hubContext,
                                    IAzureOpenAiRequestService azureOpenAiRequestService,
                                    IUserRepository userRepository,
                                    IWalletRepository walletRepository)
        {
            _userRepository = userRepository;
            _walletRepository = walletRepository;
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

        /// <inheritdoc cref="ICommunicationService.ChatRequestAsync(Guid, CreateConversationRequest)"/>
        public async Task<string?> ChatRequestAsync(Guid userId, CreateConversationRequest createConversationRequest)
        {
            await CheckIfUserAllowedToCreateRequest(userId);

            var request = new MessageRequest
            {
                Temperature = createConversationRequest.Temperature,
                Template = await GetTemplateAsync(createConversationRequest.TemplateName),
                Url = GetAiModelLink(createConversationRequest.AiModelEnum),
                UserInput = createConversationRequest.UserInput
            };
            var response = await _customAiService.PostAiRequestAsync(request);

            await _userRepository.UpdateUserTotalPointsUsageAsync(userId, response.Usage.TotalTokens / PaymentConstants.TokensToPointsRelation);
            await _walletRepository.TakeServiceFeeAsync(userId, TokensToPointsConverter(response.Usage.TotalTokens));

            return response.Content;
        }

        /// <inheritdoc cref="ICommunicationService.StreamSignalRConversationAsync(Guid, StreamRequest)"/>
        public async Task<string> StreamSignalRConversationAsync(Guid userId, StreamRequest streamRequest)
        {
            await CheckIfUserAllowedToCreateRequest(userId);

            if (!StreamAiHub.listOfConnections.Any(c => c.Equals(streamRequest.ConnectionId)))
                throw new Exception($"We can`t find connectionId => {streamRequest.ConnectionId}");

            var request = new MessageRequest
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

            var promptTokens = Tokenizer(streamRequest.AiModelEnum, $"{request.Template} {streamRequest.UserInput}");
            var completionTokens = Tokenizer(streamRequest.AiModelEnum, stringBuilder.ToString());

            var response = new CommunicationResponseModel
            {
                Content = stringBuilder.ToString(),
                Usage = new Common.Models.Usage
                {
                    PromptTokens = promptTokens,
                    CompletionTokens = completionTokens,
                    TotalTokens = promptTokens + completionTokens
                }
            };

            await _userRepository.UpdateUserTotalPointsUsageAsync(userId, response.Usage.TotalTokens / PaymentConstants.TokensToPointsRelation);
            await _walletRepository.TakeServiceFeeAsync(userId, TokensToPointsConverter(response.Usage.TotalTokens));

            return response.Content;
        }

        /// <inheritdoc cref="ICommunicationService.ChatRequestWithFunctionAsync(Guid, CreateConversationRequest)"/>
        public async Task<string?> ChatRequestWithFunctionAsync(Guid userId, CreateConversationRequest createConversationRequest)
        {
            await CheckIfUserAllowedToCreateRequest(userId);

            var request = new MessageRequest
            {
                Functions = await GetFunctionsAsync(createConversationRequest.TemplateName) ?? throw new Exception("Functions is null, change method or update the prompt"),
                Temperature = createConversationRequest.Temperature,
                Template = await GetTemplateAsync(createConversationRequest.TemplateName),
                Url = GetAiModelLink(createConversationRequest.AiModelEnum),
                UserInput = createConversationRequest.UserInput
            };
            var response = await _customAiService.PostAiRequestWithFunctionAsync(request);

            await _userRepository.UpdateUserTotalPointsUsageAsync(userId, response.Usage.TotalTokens / PaymentConstants.TokensToPointsRelation);
            await _walletRepository.TakeServiceFeeAsync(userId, TokensToPointsConverter(response.Usage.TotalTokens));

            return response.Content;
        }

        private int Tokenizer(AiModelEnum aiModelEnum, string text)
        {
            var model = string.Empty;

            if (aiModelEnum == AiModelEnum.Turbo)
                model = "gpt-3.5-turbo";

            if (aiModelEnum == AiModelEnum.Four)
                model = "gpt-4";

            var encoding = Tiktoken.Encoding.ForModel(model);
            var tokens = encoding.CountTokens(text);

            return tokens;
        }

        private int TokensToPointsConverter(int amountOfTokens)
        {
            return amountOfTokens / PaymentConstants.TokensToPointsRelation;
        }

        private async Task CheckIfUserAllowedToCreateRequest(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new Exception("User was not found");

            var minBalanceAllowedToUser = (int)((user.TotalPurchasedPoints * 0.25f) - user.TotalPointsUsage);

            if (await _walletRepository.CheckIfUserAllowedToCreateRequest(userId, minBalanceAllowedToUser))
                throw new Exception("You need to replenish your balance in order to perform further requests");
        }
    }
}
