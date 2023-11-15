using AICommunicationService.Common.Models.AIRequest;

namespace AICommunicationService.BLL.Interfaces
{
    /// <summary>
    /// This interface defines methods for communication with Chat GPT using different templates.
    /// </summary>
    public interface ICommunicationService
    {
        /// <summary>
        /// Endpoint for retrieving AI response.
        /// </summary>
        /// <param name="createConversationRequest">Request params</param>
        /// <returns>Returns string data</returns>
        Task<string> ChatRequestAsync(CreateConversationRequest createConversationRequest);

        /// <summary>
        /// Endpoint for retrieving AI response by Json Example.
        /// </summary>
        /// <param name="createConversationRequest">Request params</param>
        /// <returns>Returns string data in Json</returns>
        Task<string> ChatRequestWithFunctionAsync(CreateConversationRequest createConversationRequest);

        /// <summary>
        /// Endpoint for retrieving AI response as streaming using SignalR.
        /// </summary>
        /// <param name="streamRequest">Request params</param>
        /// <returns>Returns response is success</returns>
        Task<string> StreamSignalRConversationAsync(StreamRequest streamRequest);
    }
}
