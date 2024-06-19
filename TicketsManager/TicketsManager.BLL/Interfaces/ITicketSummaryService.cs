using TicketsManager.BLL.Dtos.TicketSummaryDtos;

namespace TicketsManager.BLL.Interfaces;

public interface ITicketSummaryService
{
    /// <summary>
    ///     Get ticket summaries by ticket id
    /// </summary>
    /// <param name="ticketId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<TicketSummaryFullDto>> GetTicketSummariesByTicketIdAsync(Guid userId, Guid ticketId);

    /// <summary>
    ///     Create ticket summaries
    /// </summary>
    /// <param name="ticketSummaries"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<TicketSummaryFullDto>> CreateTicketSummariesAsync(
        Guid userId, List<TicketSummaryCreateDto> ticketSummaries);

    /// <summary>
    ///     Update ticket summaries
    /// </summary>
    /// <param name="ticketSummaries"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<TicketSummaryFullDto>> UpdateTicketSummariesAsync(
        Guid userId, List<TicketSummaryFullDto> ticketSummaries);

    /// <summary>
    ///     Delete ticket summaries
    /// </summary>
    /// <param name="ticketId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task DeleteTicketSummariesAsync(Guid userId, Guid ticketId);
}