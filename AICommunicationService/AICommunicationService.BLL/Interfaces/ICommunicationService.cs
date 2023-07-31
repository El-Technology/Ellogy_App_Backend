using AICommunicationService.Common.Models;

namespace AICommunicationService.BLL.Interfaces
{
    public interface ICommunicationService
    {
        Task<DescriptionResponse> GetDescriptionAsync(string userStories);
        Task<DiagramResponse> GetDiagramsAsync(string userStories);
        Task<bool> GetIsRequestClearAsync(string history);
        Task<List<PotentialSummaryResponse>> GetPotentialSummaryAsync(string description);
        Task<string> SendMessageAsync(string message);
    }
}
