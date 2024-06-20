using TicketsManager.BLL.Dtos.ActionHistoryDtos;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.BLL.Interfaces;

public interface IActionHistoryService
{
    /// <summary>
    /// Creates new action history record
    /// </summary>
    /// <param name="createActionHistoryDto"></param>
    /// <returns>Status code</returns>
    Task CreateActionHistoryAsync(CreateActionHistoryDto createActionHistoryDto);

    /// <summary>
    /// Returns ticket histories by search params
    /// </summary>
    /// <param name="ticketId"></param>
    /// <param name="searchHistoryRequestDto"></param>
    /// <returns>Response with pagination ActionHistory model</returns>
    Task<PaginationResponseDto<ActionHistory>> GetActionHistoriesAsync(
        Guid ticketId, SearchHistoryRequestDto searchHistoryRequestDto);
}
