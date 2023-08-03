using AICommunicationService.DAL.Models;

namespace AICommunicationService.DAL.Interfaces
{
    /// <summary>
    /// Interface for accessing and managing AI prompt-related data in the repository.
    /// </summary>
    public interface IAIPromptRepository
    {
        /// <summary>
        /// Add a new AI prompt to the repository.
        /// </summary>
        /// <param name="aiPrompt">The AI prompt data to be added.</param>
        /// <returns>Returns a task representing the asynchronous add operation.</returns>
        Task AddPromptAsync(AIPrompt aiPrompt);

        /// <summary>
        /// Delete an existing AI prompt from the repository.
        /// </summary>
        /// <param name="aiPrompt">The AI prompt data to be deleted.</param>
        /// <returns>Returns a task representing the asynchronous delete operation.</returns>
        Task DeletePromptAsync(AIPrompt aiPrompt);

        /// <summary>
        /// Retrieve a list of all AI prompts available in the repository.
        /// </summary>
        /// <returns>Returns a task representing the asynchronous operation and a list of all AI prompts.</returns>
        Task<List<AIPrompt>> GetAllPromptsAsync();

        /// <summary>
        /// Retrieve an AI prompt from the repository based on the provided template name.
        /// </summary>
        /// <param name="templateName">The template name of the AI prompt to be retrieved.</param>
        /// <returns>Returns a task representing the asynchronous operation and the retrieved AI prompt, if found; otherwise, null.</returns>
        Task<AIPrompt?> GetPromptByTemplateNameAsync(string templateName);

        /// <summary>
        /// Update an existing AI prompt in the repository.
        /// </summary>
        /// <param name="aiPrompt">The updated AI prompt data.</param>
        /// <returns>Returns a task representing the asynchronous update operation.</returns>
        Task UpdatePromptAsync(AIPrompt aIPrompt);
    }
}
