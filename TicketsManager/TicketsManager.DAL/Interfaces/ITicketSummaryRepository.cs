using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Models.TicketSummaryModels;

namespace TicketsManager.DAL.Interfaces;

public interface ITicketSummaryRepository
{
    /// <summary>
    ///     Create ticket summaries with included scenarios and acceptance criteria
    /// </summary>
    /// <param name="ticketSummaries"></param>
    /// <returns></returns>
    Task CreateTicketSummariesAsync(List<TicketSummary> ticketSummaries);

    /// <summary>
    ///     Get ticket summaries by ticket id
    /// </summary>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    IQueryable<TicketSummary> GetTicketSummariesByTicketIdAsync(Guid ticketId);

    /// <summary>
    ///     Update ticket summaries with included scenarios and acceptance criteria
    /// </summary>
    /// <param name="ticketSummaries"></param>
    /// <returns></returns>
    Task UpdateTicketSummariesAsync(List<TicketSummary> ticketSummaries);

    /// <summary>
    ///     Delete ticket summaries by ticket id
    /// </summary>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    Task DeleteTicketSummariesAsync(Guid ticketId);
    Task DeleteTicketSummaryScenariosAsync(List<Guid> summaryScenarioIds);
    Task DeleteTicketSummaryAcceptanceCriteriaAsync(List<Guid> summaryAcceptanceCriteriaIds);
    Task DeleteTicketSummariesByIdAsync(List<Guid> summaryIds);

    /// <summary>
    ///    Get ticket summaries by ticket id with pagination
    /// </summary>
    /// <param name="ticketId"></param>
    /// <param name="paginationRequestDto"></param>
    /// <returns></returns>
    Task<PaginationResponseDto<TicketSummary>> GetTicketSummariesAsync(
        Guid ticketId, PaginationRequestDto paginationRequestDto);
}