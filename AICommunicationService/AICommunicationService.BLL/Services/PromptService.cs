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
            if (await _aIPromptRepository.GetPromptByTemplateNameAsync(aIPrompt.TamplateName) is not null)
                throw new Exception("Prompt with this name already exists");

            await _aIPromptRepository.AddPromptAsync(aIPrompt);
            return aIPrompt;
        }

        /// <inheritdoc cref="IPromptService.UpdatePromptAsync(UpdateDeletePrompt, string)"/>
        public async Task<AIPrompt> UpdatePromptAsync(UpdateDeletePrompt aIPrompt, string promptName)
        {
            var prompt = await _aIPromptRepository.GetPromptByTemplateNameAsync(promptName)
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

        /// <inheritdoc cref="IPromptService.DeletePromptAsync(UpdateDeletePrompt, string)"/>
        public async Task<AIPrompt> DeletePromptAsync(UpdateDeletePrompt aIPrompt, string promptName)
        {
            var prompt = await _aIPromptRepository.GetPromptByTemplateNameAsync(promptName)
                                       ?? throw new Exception($"Prompt with name {promptName} was not found");
            prompt.Value = aIPrompt.Value;

            await _aIPromptRepository.DeletePromptAsync(prompt);
            return prompt;
        }
    }
}
