using AICommunicationService.DAL.Context;
using AICommunicationService.DAL.Interfaces;
using AICommunicationService.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace AICommunicationService.DAL.Repositories
{
    public class AIPromptRepository : IAIPromptRepository
    {
        private readonly AICommunicationContext _context;
        public AIPromptRepository(AICommunicationContext context)
        {
            _context = context;
        }

        /// <inheritdoc cref="IAIPromptRepository.GetAllPromptsAsync"/>
        public async Task<List<AIPrompt>> GetAllPromptsAsync()
        {
            return await _context.AIPrompts.ToListAsync();
        }

        /// <inheritdoc cref="IAIPromptRepository.GetPromptByTemplateNameAsync(string)"/>
        public async Task<AIPrompt?> GetPromptByTemplateNameAsync(string templateName)
        {
            return await _context.AIPrompts.AsTracking().FirstOrDefaultAsync(a => a.TamplateName.Equals(templateName));
        }

        /// <inheritdoc cref="IAIPromptRepository.UpdatePromptAsync(AIPrompt)"/>
        public async Task UpdatePromptAsync(AIPrompt aIPrompt)
        {
            _context.AIPrompts.Update(aIPrompt);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc cref="IAIPromptRepository.AddPromptAsync(AIPrompt)"/>
        public async Task AddPromptAsync(AIPrompt aiPrompt)
        {
            await _context.AIPrompts.AddAsync(aiPrompt);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc cref="IAIPromptRepository.DeletePromptAsync(AIPrompt)"/>
        public async Task DeletePromptAsync(AIPrompt aiPrompt)
        {
            _context.AIPrompts.Remove(aiPrompt);
            await _context.SaveChangesAsync();
        }
    }
}
