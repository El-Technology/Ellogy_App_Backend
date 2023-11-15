namespace AICommunicationService.DAL.Interfaces
{
    public interface ITicketRepository
    {
        Task<int?> UpdateTokensUsageAsync(Guid ticketId, int usedTokens);
    }
}
