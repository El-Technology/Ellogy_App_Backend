using AICommunicationService.BLL.Dtos;
using AICommunicationService.DAL.Models;

namespace AICommunicationService.BLL.Interfaces
{
    /// <summary>
    /// Interface for managing prompt-related operations.
    /// </summary>
    public interface IPromptService
    {
        /// <summary>
        /// Add a new prompt using the provided data.
        /// </summary>
        /// <param name="aIPrompt">The data required to create the prompt.</param>
        /// <returns>Returns a task representing the asynchronous add operation.</returns>
        Task<AIPrompt> AddPromptAsync(CreatePromptDto aIPrompt);

        /// <summary>
        /// Delete a prompt with the specified name using the provided data.
        /// </summary>
        /// <param name="aIPrompt">The data required to delete the prompt.</param>
        /// <param name="promptName">The name of the prompt to be deleted.</param>
        /// <returns>Returns a task representing the asynchronous delete operation.</returns>
        Task<AIPrompt> DeletePromptAsync(UpdateDeletePrompt aIPrompt, string promptName);

        /// <summary>
        /// Retrieve a list of all prompts available in the system.
        /// </summary>
        /// <returns>Returns a task representing the asynchronous operation and a list of all prompts.</returns>
        Task<List<AIPrompt>> GetAllPromptsAsync();

        /// <summary>
        /// Update an existing prompt with the specified name using the provided data.
        /// </summary>
        /// <param name="aIPrompt">The updated data for the prompt.</param>
        /// <param name="promptName">The name of the prompt to be updated.</param>
        /// <returns>Returns a task representing the asynchronous update operation.</returns>
        Task<AIPrompt> UpdatePromptAsync(UpdateDeletePrompt aIPrompt, string promptName);
    }
}
