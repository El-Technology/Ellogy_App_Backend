using AICommunicationService.Common.Models.AIRequest;

namespace AICommunicationService.BLL.Interfaces;

/// <summary>
/// This interface defines methods for communication with Chat GPT using different templates.
/// </summary>
public interface ICommunicationService
{
    /// <summary>
    /// Endpoint for retrieving AI response.
    /// </summary>
    /// <param name="createConversationRequest">Request params</param>
    /// <param name="userId">User id</param>
    /// <returns>Returns string data</returns>
    Task<string> ChatRequestAsync(Guid userId, CreateConversationRequest createConversationRequest);

    /// <summary>
    /// Endpoint for retrieving AI response by Json Example.
    /// </summary>
    /// <param name="createConversationRequest">Request params</param>
    /// <param name="userId">User id</param>
    /// <returns>Returns string data in Json</returns>
    Task<string> ChatRequestWithFunctionAsync(Guid userId, CreateConversationRequest createConversationRequest);

    /// <summary>
    /// Endpoint for retrieving AI response as streaming
    /// </summary>
    /// <param name="createConversationRequest">Request params</param>
    /// <param name="userId">User id</param>
    /// <returns>Returns response is success</returns>
    Task StreamRequestAsync(Guid userId, CreateConversationRequest createConversationRequest, Func<string, Task> onDataReceived);
}
