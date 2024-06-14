using AICommunicationService.BLL.Constants;
using AICommunicationService.BLL.Dtos;
using AICommunicationService.BLL.Exceptions;
using AICommunicationService.BLL.Interfaces;
using AICommunicationService.BLL.Interfaces.HttpInterfaces;
using AICommunicationService.Common;
using AICommunicationService.Common.Enums;
using AICommunicationService.Common.Models.AIRequest;
using AICommunicationService.Common.Models.GptResponseModel;
using AICommunicationService.DAL.Interfaces;
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
    private readonly IUserExternalHttpService _userExternalHttpService;
    private readonly IPaymentExternalHttpService _paymentExternalHttpService;
    private readonly IGroqAiRequestService _groqAiRequestService;

    public CommunicationService(IAIPromptRepository aIPromptRepository,
        IAzureOpenAiRequestService azureOpenAiRequestService,
        IDocumentService documentService,
        IUserExternalHttpService userExternalHttpService,
        IPaymentExternalHttpService paymentExternalHttpService,
        IGroqAiRequestService groqAiRequestService)
    {
        _groqAiRequestService = groqAiRequestService;
        _aIPromptRepository = aIPromptRepository;
        _customAiService = azureOpenAiRequestService;
        _documentService = documentService;
        _userExternalHttpService = userExternalHttpService;
        _paymentExternalHttpService = paymentExternalHttpService;
    }

    /// <inheritdoc cref="ICommunicationService.ChatRequestAsync(Guid, ConversationRequestDto)" />
    public async Task<string> ChatRequestAsync(Guid userId, ConversationRequestDto conversationRequest)
    {
        var request = await CreateMessageRequestAsync(userId, conversationRequest, AiRequestType.Default);

        var response = (int)conversationRequest.CreateConversationRequest.AiModelEnum switch
        {
            (int)AiModelEnum.Mixtral_8x7b or (int)AiModelEnum.Llama3_70b =>
                await _groqAiRequestService.PostAiRequestAsync(
                    request, AiRequestType.Default, conversationRequest.CreateConversationRequest.AiModelEnum),

            _ => await _customAiService.PostAiRequestAsync(request)
        };

        return await ProcessResultAsync(
            userId, response, conversationRequest.CreateConversationRequest.TemplateName, conversationRequest.AccountPlan);
    }

    /// <inheritdoc cref="ICommunicationService.ChatRequestWithFunctionAsync(Guid, ConversationRequestDto)" />
    public async Task<string> ChatRequestWithFunctionAsync(Guid userId, ConversationRequestDto conversationRequest)
    {
        var request = await CreateMessageRequestAsync(userId, conversationRequest, AiRequestType.Functions);

        var response = (int)conversationRequest.CreateConversationRequest.AiModelEnum switch
        {
            (int)AiModelEnum.Mixtral_8x7b or (int)AiModelEnum.Llama3_70b =>
                await _groqAiRequestService.PostAiRequestAsync(
                    request, AiRequestType.Functions, conversationRequest.CreateConversationRequest.AiModelEnum),

            _ => await _customAiService.PostAiRequestWithFunctionAsync(request)
        };

        return await ProcessResultAsync(
            userId, response, conversationRequest.CreateConversationRequest.TemplateName, conversationRequest.AccountPlan);
    }

    /// <inheritdoc cref="ICommunicationService.StreamRequestAsync(Guid, ConversationRequestDto, Func{string, Task})" />
    public async Task StreamRequestAsync(
    Guid userId, ConversationRequestDto conversationRequest, Func<string, Task> onDataReceived)
    {
        var request = await CreateMessageRequestAsync(userId, conversationRequest, AiRequestType.Streaming);
        var stringBuilder = new StringBuilder();

        await _customAiService.PostAiRequestAsStreamAsync(request,
            async response =>
            {
                await onDataReceived(response);
                stringBuilder.Append(response);
            });

        var promptTokens = Tokenizer(conversationRequest.CreateConversationRequest.AiModelEnum,
            $"{request.Template} {conversationRequest.CreateConversationRequest.UserInput}");
        var completionTokens = Tokenizer(conversationRequest.CreateConversationRequest.AiModelEnum,
            stringBuilder.ToString());

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

        await ProcessResultAsync(
            userId, response, conversationRequest.CreateConversationRequest.TemplateName, conversationRequest.AccountPlan);
    }

    private async Task<MessageRequest> CreateMessageRequestAsync(
        Guid userId, ConversationRequestDto conversationRequest, AiRequestType requestType)
    {
        if (!(conversationRequest.AccountPlan == AccountPlan.Starter))
            await CheckIfUserAllowedToCreateRequest(userId);

        var createConversationRequest = conversationRequest.CreateConversationRequest;

        return new MessageRequest
        {
            Context = await GetRAGContextAsync(userId, createConversationRequest),
            Functions = (requestType == AiRequestType.Functions)
                    ? await GetFunctionsAsync(createConversationRequest.TemplateName, createConversationRequest.AiModelEnum)
                    : null,
            ConversationHistory = createConversationRequest.ConversationHistory,
            Temperature = createConversationRequest.Temperature,
            Template = await GetTemplateAsync(createConversationRequest.TemplateName),
            Url = await GetAzureOpenAiRequestLinkAsync(createConversationRequest.AiModelEnum),
            UserInput = createConversationRequest.UserInput
        };
    }

    private async Task<string> ProcessResultAsync(Guid userId, CommunicationResponseModel response, string templateName, AccountPlan accountPlan)
    {
        var isTemplateFree = TemplatesWithoutChargeConst.ListOfFreeTemplates.Any(a => a.Equals(templateName));
        var isPlanRequireCharge = accountPlan != AccountPlan.Starter;

        if (!isTemplateFree && isPlanRequireCharge)
            await TakeChargeAsync(userId, response);

        return response.Content ?? string.Empty;
    }

    private int Tokenizer(AiModelEnum aiModelEnum, string text)
    {
        var model = aiModelEnum switch
        {
            AiModelEnum.Turbo => "gpt-3.5-turbo",
            _ => "gpt-4"
        };

        var encoding = Encoding.ForModel(model);
        var tokens = encoding.CountTokens(text);

        return tokens;
    }

    private int TokensToPointsConverter(int? amountOfTokens) =>
        (amountOfTokens ?? 0) / PaymentConstants.TokensToPointsRelation;

    private async Task TakeChargeAsync(Guid userId, CommunicationResponseModel response)
    {
        if (!bool.Parse(await EnvironmentVariables.EnablePayments)) return;

        ArgumentNullException.ThrowIfNull(response.Usage, "Usage is null");

        await _userExternalHttpService.UpdateUserTotalPointsUsageAsync(userId,
            response.Usage.TotalTokens / PaymentConstants.TokensToPointsRelation);
        await _paymentExternalHttpService.TakeServiceFeeAsync(userId, TokensToPointsConverter(response.Usage.TotalTokens));
    }

    private async Task CheckIfUserAllowedToCreateRequest(Guid userId)
    {
        if (!bool.Parse(await EnvironmentVariables.EnablePayments)) return;

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

    private async Task<string?> GetFunctionsAsync(string promptName, AiModelEnum aiModelEnum)
    {
        var getPrompt = await _aIPromptRepository.GetPromptByTemplateNameAsync(promptName)
                        ?? throw new Exception("Prompt was not found");

        var response = (int)aiModelEnum switch
        {
            (int)AiModelEnum.Mixtral_8x7b or (int)AiModelEnum.Llama3_70b => getPrompt.JsonSample,
            _ => getPrompt.Functions
        };

        return response;
    }

    private async Task<string> GetAzureOpenAiRequestLinkAsync(AiModelEnum aiModelEnum)
    {
        if ((int)aiModelEnum == (int)AiModelEnum.Mixtral_8x7b || (int)aiModelEnum == (int)AiModelEnum.Llama3_70b)
            return string.Empty;

        var deploymentName = aiModelEnum switch
        {
            AiModelEnum.Turbo => AzureAiConstants.TurboModel,
            AiModelEnum.Four => AzureAiConstants.FourModel,
            AiModelEnum.FourTurbo => AzureAiConstants.FourTurboModel,
            AiModelEnum.Four32k => AzureAiConstants.Four32kModel,
            AiModelEnum.FourO => AzureAiConstants.FourOModel,
            _ => throw new Exception("Wrong enum")
        };
        return $"{AzureAiConstants.BaseUrl}{deploymentName}/chat/completions?{AzureAiConstants.ApiVersion}";
    }

    private async Task<string?> GetRAGContextAsync(Guid userId, CreateConversationRequest createConversationRequest)
    {
        if (createConversationRequest is not { UseRAG: true, FileName: not null, RagTemplate: not null })
            return null;

        var getPrompt = await _aIPromptRepository.GetPromptByTemplateNameAsync(createConversationRequest.RagTemplate)
                        ?? throw new Exception("Prompt was not found");

        var ragContext = await _documentService.GetTheClosesContextAsync(userId, createConversationRequest.UserInput,
            createConversationRequest.FileName);

        return $"{getPrompt.Value} {ragContext}";
    }
}