using TicketsManager.BLL.Dtos.TicketSummaryDtos;

namespace TicketsManager.BLL.Interfaces;

public interface ITicketSummaryService
{
    /// <summary>
    ///     Get ticket summaries by ticket id
    /// </summary>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    Task<List<TicketSummaryFullDto>> GetTicketSummariesByTicketIdAsync(Guid ticketId);

    /// <summary>
    ///     Create ticket summaries
    /// </summary>
    /// <param name="ticketSummaries"></param>
    /// <returns></returns>
    Task<List<TicketSummaryFullDto>> CreateTicketSummariesAsync(
        List<TicketSummaryCreateDto> ticketSummaries);

    /// <summary>
    ///     Update ticket summaries
    /// </summary>
    /// <param name="ticketSummaries"></param>
    /// <returns></returns>
    Task<List<TicketSummaryFullDto>> UpdateTicketSummariesAsync(
        List<TicketSummaryFullDto> ticketSummaries);

    /// <summary>
    ///     Delete ticket summaries
    /// </summary>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    Task DeleteTicketSummariesAsync(Guid ticketId);
}