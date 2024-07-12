using TicketsManager.BLL.Dtos.TicketSummaryDtos;
using TicketsManager.Common.Dtos;

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
    Task DeleteTicketSummaryScenariosAsync(Guid userId, Guid ticketId, List<Guid> summaryScenarioIds);
    Task DeleteTicketSummaryAcceptanceCriteriaAsync(Guid userId, Guid ticketId, List<Guid> summaryAcceptanceCriteriaIds);
    Task DeleteTicketSummariesByIdsAsync(Guid userId, Guid ticketId, List<Guid> summaryIds);
    Task<PaginationResponseDto<TicketSummaryFullDto>> GetTicketSummariesByTicketIdAsync(Guid userId, Guid ticketId, PaginationRequestDto paginationRequestDto);
}