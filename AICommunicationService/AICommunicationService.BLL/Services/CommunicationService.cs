using AICommunicationService.BLL.Interfaces;
using OpenAI_API;

namespace AICommunicationService.BLL.Services
{
    public class CommunicationService : ICommunicationService
    {
        private readonly OpenAIAPI _openAIAPI;
        public CommunicationService(OpenAIAPI openAIAPI)
        {
            _openAIAPI = openAIAPI;
        }

        public async Task<string> SendMessageAsync(string message)
        {
            var result = await _openAIAPI.Completions.GetCompletion(message);


            return result;
        }
    }
}
