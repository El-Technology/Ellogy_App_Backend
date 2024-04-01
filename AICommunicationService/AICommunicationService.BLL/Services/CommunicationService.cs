using AICommunicationService.BLL.Constants;
using AICommunicationService.BLL.Dtos;
using AICommunicationService.BLL.Exceptions;
using AICommunicationService.BLL.Hubs;
using AICommunicationService.BLL.Interfaces;
using AICommunicationService.BLL.Interfaces.HttpInterfaces;
using AICommunicationService.Common;
using AICommunicationService.Common.Constants;
using AICommunicationService.Common.Enums;
using AICommunicationService.Common.Models;
using AICommunicationService.Common.Models.AIRequest;
using AICommunicationService.DAL.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Text;
using Encoding = Tiktoken.Encoding;

namespace AICommunicationService.BLL.Services;

/// <summary>
///     This class provides an implementation for communication with Chat GPT using different templates. It utilizes the
///     methods defined in the <see cref="ICommunicationService" /> interface.
/// </summary>
public class CommunicationService : ICommunicationService
{
    private readonly IAIPromptRepository _aIPromptRepository;
    private readonly IAzureOpenAiRequestService _customAiService;
    private readonly IDocumentService _documentService;
    private readonly IHubContext<StreamAiHub> _hubContext;
    private readonly IUserExternalHttpService _userExternalHttpService;
    private readonly IPaymentExternalHttpService _paymentExternalHttpService;

    public CommunicationService(IAIPromptRepository aIPromptRepository,
        IHubContext<StreamAiHub> hubContext,
        IAzureOpenAiRequestService azureOpenAiRequestService,
        IDocumentService documentService,
        IUserExternalHttpService userExternalHttpService,
        IPaymentExternalHttpService paymentExternalHttpService)
    {
        _hubContext = hubContext;
        _aIPromptRepository = aIPromptRepository;
        _customAiService = azureOpenAiRequestService;
        _documentService = documentService;
        _userExternalHttpService = userExternalHttpService;
        _paymentExternalHttpService = paymentExternalHttpService;
    }

    /// <inheritdoc cref="ICommunicationService.ChatRequestAsync(Guid, CreateConversationRequest)" />
    public async Task<string?> ChatRequestAsync(Guid userId, CreateConversationRequest createConversationRequest)
    {
        await CheckIfUserAllowedToCreateRequest(userId);

        var request = new MessageRequest
        {
            Context = await GetRAGContextAsync(userId, createConversationRequest),
            Temperature = createConversationRequest.Temperature,
            Template = await GetTemplateAsync(createConversationRequest.TemplateName),
            Url = GetAiModelLink(createConversationRequest.AiModelEnum),
            UserInput = createConversationRequest.UserInput
        };
        var response = await _customAiService.PostAiRequestAsync(request);

        await TakeChargeAsync(userId, response);

        return response.Content;
    }

    /// <inheritdoc cref="ICommunicationService.StreamSignalRConversationAsync(Guid, StreamRequest)" />
    public async Task<string> StreamSignalRConversationAsync(Guid userId, StreamRequest streamRequest)
    {
        await CheckIfUserAllowedToCreateRequest(userId);

        if (!StreamAiHub.listOfConnections.Any(c => c.Equals(streamRequest.ConnectionId)))
            throw new Exception($"We can`t find connectionId => {streamRequest.ConnectionId}");

        var request = new MessageRequest
        {
            Context = await GetRAGContextAsync(userId, streamRequest),
            Temperature = streamRequest.Temperature,
            Template = await GetTemplateAsync(streamRequest.TemplateName),
            Url = GetAiModelLink(streamRequest.AiModelEnum),
            UserInput = streamRequest.UserInput
        };

        var stringBuilder = new StringBuilder();
        await _customAiService.PostAiRequestAsStreamAsync(request, async response =>
        {
            await _hubContext.Clients.Client(streamRequest.ConnectionId)
                .SendAsync(streamRequest.SignalMethodName, response);
            stringBuilder.Append(response);
        });

        var promptTokens = Tokenizer(streamRequest.AiModelEnum, $"{request.Template} {streamRequest.UserInput}");
        var completionTokens = Tokenizer(streamRequest.AiModelEnum, stringBuilder.ToString());

        var response = new CommunicationResponseModel
        {
            Content = stringBuilder.ToString(),
            Usage = new Usage
            {
                PromptTokens = promptTokens,
                CompletionTokens = completionTokens,
                TotalTokens = promptTokens + completionTokens
            }
        };

        await TakeChargeAsync(userId, response);

        return response.Content;
    }

    /// <inheritdoc cref="ICommunicationService.ChatRequestWithFunctionAsync(Guid, CreateConversationRequest)" />
    public async Task<string?> ChatRequestWithFunctionAsync(Guid userId,
        CreateConversationRequest createConversationRequest)
    {
        await CheckIfUserAllowedToCreateRequest(userId);

        var request = new MessageRequest
        {
            Context = await GetRAGContextAsync(userId, createConversationRequest),
            Functions = await GetFunctionsAsync(createConversationRequest.TemplateName) ??
                        throw new Exception("Functions is null, change method or update the prompt"),
            Temperature = createConversationRequest.Temperature,
            Template = await GetTemplateAsync(createConversationRequest.TemplateName),
            Url = GetAiModelLink(createConversationRequest.AiModelEnum),
            UserInput = createConversationRequest.UserInput
        };
        var response = await _customAiService.PostAiRequestWithFunctionAsync(request);

        await TakeChargeAsync(userId, response);

        return response.Content;
    }

    private int Tokenizer(AiModelEnum aiModelEnum, string text)
    {
        var model = string.Empty;

        if (aiModelEnum == AiModelEnum.Turbo)
            model = "gpt-3.5-turbo";

        if (aiModelEnum is AiModelEnum.Four or AiModelEnum.FourTurbo or AiModelEnum.Four32k)
            model = "gpt-4";

        var encoding = Encoding.ForModel(model);
        var tokens = encoding.CountTokens(text);

        return tokens;
    }

    private int TokensToPointsConverter(int amountOfTokens)
    {
        return amountOfTokens / PaymentConstants.TokensToPointsRelation;
    }

    private async Task TakeChargeAsync(Guid userId, CommunicationResponseModel response)
    {
        if (!EnvironmentVariables.EnablePayments)
            return;

        await _userExternalHttpService.UpdateUserTotalPointsUsageAsync(userId,
            response.Usage.TotalTokens / PaymentConstants.TokensToPointsRelation);
        await _paymentExternalHttpService.TakeServiceFeeAsync(userId, TokensToPointsConverter(response.Usage.TotalTokens));
    }

    private async Task CheckIfUserAllowedToCreateRequest(Guid userId)
    {
        if (!EnvironmentVariables.EnablePayments)
            return;

        var user = await _userExternalHttpService.GetUserByIdAsync(userId)
                   ?? throw new Exception("User was not found");

        var minBalanceAllowedToUser = (int)-(user.TotalPurchasedPoints * 0.2f);
        if (minBalanceAllowedToUser > 0)
            return;

        if (await _paymentExternalHttpService.CheckIfUserAllowedToCreateRequestAsync(userId, minBalanceAllowedToUser))
            throw new BalanceException("You need to replenish your balance in order to perform further requests");
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
        var deploymentName = aiModelEnum switch
        {
            AiModelEnum.Turbo => AzureAiConstants.TurboModel,
            AiModelEnum.Four => AzureAiConstants.FourModel,
            AiModelEnum.FourTurbo => AzureAiConstants.FourTurboModel,
            AiModelEnum.Four32k => AzureAiConstants.Four32kModel,
            _ => throw new Exception("Wrong enum")
        };
        return $"{AzureAiConstants.BaseUrl}{deploymentName}/chat/completions?{AzureAiConstants.ApiVersion}";
    }

    private async Task<string?> GetRAGContextAsync(Guid userId, CreateConversationRequest createConversationRequest)
    {
        if (createConversationRequest is not { UseRAG: true, FileName: not null })
            return null;

        var getPrompt = await _aIPromptRepository.GetPromptByTemplateNameAsync(RagConstants.RAG_CONTEXT)
                        ?? throw new Exception("Prompt was not found");

        var ragContext = await _documentService.GetTheClosesContextAsync(userId, createConversationRequest.UserInput,
            createConversationRequest.FileName);

        return $"{getPrompt.Value} {ragContext}";
    }
}