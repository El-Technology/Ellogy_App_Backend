using AICommunicationService.DAL.Interfaces;
using AICommunicationService.DAL.Models;

namespace AICommunicationService.DAL.Repositories
{
    public class AIPromptRepository : IAIPromptRepository
    {
        private readonly IDapperRepository _dapperRepository;

        public AIPromptRepository(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        /// <inheritdoc cref="IAIPromptRepository.GetAllPromptsAsync"/>
        public async Task<List<AIPrompt>> GetAllPromptsAsync()
        {
            var sql = @"SELECT *
                        FROM ""AIPrompts""";

            return (await _dapperRepository.QueryAsync<AIPrompt>(sql)).ToList();
        }

        /// <inheritdoc cref="IAIPromptRepository.GetPromptByNameAsync(string)"/>
        public async Task<AIPrompt> GetPromptByNameAsync(string promptName)
        {
            var sql = @$"SELECT *
                        FROM ""AIPrompts""
                        WHERE ""TemplateName"" = @TemplateName";

            return await _dapperRepository.QueryFirstOrDefaultAsync<AIPrompt>(sql, new { TemplateName = promptName });
        }

        /// <inheritdoc cref="IAIPromptRepository.UpdatePromptAsync(AIPrompt)"/>
        public async Task UpdatePromptAsync(AIPrompt aIPrompt)
        {
            var sql = @$"UPDATE ""AIPrompts""
                        SET ""Value"" = @Value
                        WHERE ""TemplateName"" = @TemplateName";

            await _dapperRepository.ExecuteAsync(sql, aIPrompt);
        }

        /// <inheritdoc cref="IAIPromptRepository.AddPromptAsync(AIPrompt)"/>
        public async Task AddPromptAsync(AIPrompt aiPrompt)
        {
            var sql = @$"INSERT INTO ""AIPrompts""
                        VALUES (@TemplateName, @Value)";

            await _dapperRepository.ExecuteAsync(sql, aiPrompt);
        }

        /// <inheritdoc cref="IAIPromptRepository.DeletePromptAsync(AIPrompt)"/>
        public async Task DeletePromptAsync(string promptName)
        {
            var sql = @$"DELETE
                        FROM ""AIPrompts""
                        WHERE ""TemplateName"" = @TemplateName";

            await _dapperRepository.ExecuteAsync(sql, new { TemplateName = promptName });
        }
    }
}
