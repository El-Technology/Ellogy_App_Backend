using AICommunicationService.BLL.Dtos;
using AICommunicationService.BLL.Interfaces;
using AICommunicationService.DAL.Interfaces;
using AICommunicationService.DAL.Models;

namespace AICommunicationService.BLL.Services
{
    public class PromptService : IPromptService
    {
        private readonly IAIPromptRepository _aIPromptRepository;
        public PromptService(IAIPromptRepository aIPromptRepository)
        {
            _aIPromptRepository = aIPromptRepository;
        }

        /// <inheritdoc cref="IPromptService.AddPromptAsync(CreatePromptDto)"/>
        public async Task<AIPrompt> AddPromptAsync(CreatePromptDto aIPrompt)
        {
            if (await _aIPromptRepository.GetPromptByNameAsync(aIPrompt.TemplateName) is not null)
                throw new Exception("Prompt with this name already exists");

            await _aIPromptRepository.AddPromptAsync(aIPrompt);
            return aIPrompt;
        }

        /// <inheritdoc cref="IPromptService.UpdatePromptAsync(UpdatePrompt, string)"/>
        public async Task<AIPrompt> UpdatePromptAsync(UpdatePrompt aIPrompt, string promptName)
        {
            var prompt = await _aIPromptRepository.GetPromptByNameAsync(promptName)
                                                   ?? throw new Exception($"Prompt with name {promptName} was not found");

            prompt.Value = aIPrompt.Value;
            await _aIPromptRepository.UpdatePromptAsync(prompt);
            return prompt;
        }

        /// <inheritdoc cref="IPromptService.GetAllPromptsAsync"/>
        public async Task<List<AIPrompt>> GetAllPromptsAsync()
        {
            return await _aIPromptRepository.GetAllPromptsAsync();
        }

        /// <inheritdoc cref="IPromptService.DeletePromptAsync(string)"/>
        public async Task<AIPrompt> DeletePromptAsync(string promptName)
        {
            var prompt = await _aIPromptRepository.GetPromptByNameAsync(promptName)
                                       ?? throw new Exception($"Prompt with name {promptName} was not found");

            await _aIPromptRepository.DeletePromptAsync(promptName);
            return prompt;
        }
    }
}
