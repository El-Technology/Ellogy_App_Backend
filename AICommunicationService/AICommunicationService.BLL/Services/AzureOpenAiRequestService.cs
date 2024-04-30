using AICommunicationService.BLL.Constants;
using AICommunicationService.BLL.Dtos;
using AICommunicationService.BLL.Exceptions;
using AICommunicationService.BLL.Interfaces;
using AICommunicationService.Common.Enums;
using AICommunicationService.Common.Models;
using AICommunicationService.Common.Models.AIRequest;
using AICommunicationService.Common.Models.GptResponseModel;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace AICommunicationService.BLL.Services;

public class AzureOpenAiRequestService : BasicRequestService, IAzureOpenAiRequestService
{
    private readonly HttpClient _httpClient;

    public AzureOpenAiRequestService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("AzureAiRequest");
    }

    private StringContent PostAiRequestGetContent(MessageRequest request, AiRequestType requestType)
    {
        var messages = GetMessages(request);
        object requestData;
        switch (requestType)
        {
            case AiRequestType.Default:
                requestData = new
                {
                    messages,
                    temperature = request.Temperature
                };
                break;
            case AiRequestType.Functions:
                var functions = JsonConvert.DeserializeObject<List<FunctionDefinition>>(request.Functions);
                requestData = new
                {
                    messages,
                    functions,
                    function_call = new
                    {
                        name = functions?.FirstOrDefault()?.Name
                    },
                    temperature = request.Temperature
                };
                break;
            case AiRequestType.Streaming:
                requestData = new
                {
                    messages,
                    temperature = request.Temperature,
                    stream = true
                };
                break;
            default:
                throw new NotImplementedException();
        }
        var jsonRequest = JsonConvert.SerializeObject(requestData);
        return new StringContent(jsonRequest, Encoding.UTF8, "application/json");
    }

    private void ThrowGptException(string? message = "empty")
    {
        throw new GptModelException($"Model error, try to replace with another one\nRequest return message: {message}");
    }

    private async Task<AiResponseModel?> SendRequestAsync(MessageRequest request, AiRequestType requestType)
    {
        var content = PostAiRequestGetContent(request, requestType);

        var result = await _httpClient.PostAsync(request.Url, content);

        if (result.StatusCode == HttpStatusCode.TooManyRequests)
        {
            result.Headers.TryGetValues("retry-after", out var values);
            throw new ToManyRequestsException(values?.FirstOrDefault());
        }

        if (result.StatusCode == HttpStatusCode.BadRequest)
        {
            var exceptionResponse = await result.Content.ReadFromJsonAsync<ModelError>();
            ThrowGptException(exceptionResponse?.Error?.Message);
        }

        return JsonConvert.DeserializeObject<AiResponseModel>(await result.Content.ReadAsStringAsync());
    }

    /// <inheritdoc cref="IAzureOpenAiRequestService.PostAiRequestWithFunctionAsync(MessageRequest)"/>
    public async Task<CommunicationResponseModel> PostAiRequestWithFunctionAsync(MessageRequest request)
    {
        var resultAsObject = await SendRequestAsync(request, AiRequestType.Functions);

        var communicationModel = new CommunicationResponseModel
        {
            Content = resultAsObject?.Choices?.FirstOrDefault()?.Message?.FunctionCall?.Arguments,
            Usage = resultAsObject?.Usage
        };

        return communicationModel;
    }

    /// <inheritdoc cref="IAzureOpenAiRequestService.PostAiRequestAsync(MessageRequest)"/>
    public async Task<CommunicationResponseModel> PostAiRequestAsync(MessageRequest request)
    {
        var resultAsObject = await SendRequestAsync(request, AiRequestType.Default);

        var communicationModel = new CommunicationResponseModel
        {
            Content = resultAsObject?.Choices?.FirstOrDefault()?.Message?.Content,
            Usage = resultAsObject?.Usage
        };

        return communicationModel;
    }

    /// <inheritdoc cref="IAzureOpenAiRequestService.PostAiRequestAsStreamAsync(MessageRequest, Func{string, Task})"/>
    public async Task PostAiRequestAsStreamAsync(MessageRequest request, Func<string, Task> onDataReceived)
    {
        var content = PostAiRequestGetContent(request, AiRequestType.Streaming);

        var message = new HttpRequestMessage { RequestUri = new Uri(request.Url), Method = HttpMethod.Post, Content = content };
        var response = await _httpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            response.Headers.TryGetValues("retry-after", out var values);
            throw new ToManyRequestsException(values?.FirstOrDefault());
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var exceptionResponse = await response.Content.ReadFromJsonAsync<ModelError>();
            ThrowGptException(exceptionResponse?.Error?.Message);
        }

        using var stream = await response.Content.ReadAsStreamAsync();
        using var streamReader = new StreamReader(stream);

        while (!streamReader.EndOfStream)
        {
            var line = await streamReader.ReadLineAsync();
            if (string.IsNullOrEmpty(line))
                continue;

            var aiResponse = new AiResponseModel();
            try
            {
                aiResponse = JsonConvert.DeserializeObject<AiResponseModel>(line.Replace("data: ", ""));
            }
            catch (Exception) { continue; };

            var stringAiResponse = aiResponse?.Choices?.FirstOrDefault()?.Delta?.Content;
            if (!string.IsNullOrEmpty(stringAiResponse))
                await onDataReceived(stringAiResponse);
        }
    }

    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        var content = new StringContent(
            JsonConvert.SerializeObject(new { input = text }),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync(AzureAiConstants.EmbeddingUrl, content);

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            response.Headers.TryGetValues("retry-after", out var values);
            throw new ToManyRequestsException(values?.FirstOrDefault());
        }

        var resultAsObject = JsonConvert.DeserializeObject<EmbeddingResponseModel>(await response.Content.ReadAsStringAsync());

        return resultAsObject.data.FirstOrDefault().embedding;
    }
}
