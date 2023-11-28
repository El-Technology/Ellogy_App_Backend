using AICommunicationService.BLL.Dtos;
using AICommunicationService.Common.Models.AIRequest;

namespace AICommunicationService.BLL.Interfaces
{
    /// <summary>
    /// Interface defining methods for interacting with Azure OpenAI services for AI requests.
    /// </summary>
    public interface IAzureOpenAiRequestService
    {
        /// <summary>
        /// Posts an AI request as a stream and handles data reception asynchronously.
        /// </summary>
        /// <param name="request">Message request object</param>
        /// <param name="onDataReceived">Callback function for handling received data</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task PostAiRequestAsStreamAsync(MessageRequest request, Func<string, Task> onDataReceived);

        /// <summary>
        /// Posts a standard AI request and retrieves the communication response.
        /// </summary>
        /// <param name="request">Message request object</param>
        /// <returns>Communication response model</returns>
        Task<CommunicationResponseModel> PostAiRequestAsync(MessageRequest request);

        /// <summary>
        /// Posts an AI request with a specific function and retrieves the communication response.
        /// </summary>
        /// <param name="request">Message request object</param>
        /// <returns>Communication response model</returns>
        Task<CommunicationResponseModel> PostAiRequestWithFunctionAsync(MessageRequest request);
    }
}
