using AICommunicationService.BLL.Constants;
using AICommunicationService.BLL.Dtos;
using AICommunicationService.BLL.Interfaces;
using AICommunicationService.Common.Enums;
using AICommunicationService.Common.Models;
using AICommunicationService.Common.Models.AIRequest;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace AICommunicationService.BLL.Services;
public class GroqAiRequestService : BasicRequestService, IGroqAiRequestService
{
    private readonly HttpClient _httpClient;

    public GroqAiRequestService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("GroqAiRequest");
    }

    private StringContent PostAiRequestGetContent(MessageRequest request, AiRequestType requestType, AiModelEnum aiModelEnum)
    {
        var messages = GetMessages(request);
        object requestData;

        var modelName = aiModelEnum switch
        {
            AiModelEnum.Mixtral_8x7b => GroqConstants.Mixtral_8x7b,
            AiModelEnum.Llama3_70b => GroqConstants.Llama3_70b,
            _ => throw new NotImplementedException()
        };

        switch (requestType)
        {
            case AiRequestType.Default:
                requestData = new
                {
                    messages,
                    temperature = request.Temperature,
                    model = modelName,
                };
                break;
            case AiRequestType.Functions:
                messages.Insert(0, new { role = "system", content = request.Functions });

                requestData = new
                {
                    messages,
                    model = modelName,
                    response_format = new { type = "json_object" },
                    temperature = request.Temperature
                };
                break;
            default:
                throw new NotImplementedException();
        }
        var jsonRequest = JsonConvert.SerializeObject(requestData);
        return new StringContent(jsonRequest, Encoding.UTF8, "application/json");
    }

    public async Task<CommunicationResponseModel> PostAiRequestAsync(MessageRequest request, AiRequestType aiRequestType, AiModelEnum aiModelEnum)
    {
        var content = PostAiRequestGetContent(request, aiRequestType, aiModelEnum);
        var response = await _httpClient.PostAsync(GroqConstants.GroqUrl, content);

        ValidateResponse(response);

        var responseContent = await response.Content.ReadFromJsonAsync<GroqResponseModel>();

        var communicationModel = new CommunicationResponseModel
        {
            Content = responseContent?.choices.FirstOrDefault()?.message.content,
            Usage = new Common.Models.GptResponseModel.Usage
            {
                CompletionTokens = responseContent?.usage.completion_tokens ?? 0,
                PromptTokens = responseContent?.usage.prompt_tokens ?? 0,
                TotalTokens = responseContent?.usage.total_tokens ?? 0
            }
        };

        return communicationModel;
    }
}
