using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.DAL.Interfaces;

public interface IActionHistoryRepository
{
    /// <summary>
    /// Create action history
    /// </summary>
    /// <param name="actionHistory"></param>
    /// <returns></returns>
    Task CreateActionHistoryAsync(ActionHistory actionHistory);

    /// <summary>
    /// Get action histories by ticket id and ticket current step enum
    /// </summary>
    /// <param name="ticketId"></param>
    /// <param name="ticketCurrentStepEnum"></param>
    /// <param name="paginationRequest"></param>
    /// <returns></returns>
    Task<PaginationResponseDto<ActionHistory>> GetActionHistoriesAsync(
        Guid ticketId, TicketCurrentStepEnum ticketCurrentStepEnum, PaginationRequestDto paginationRequest);
}
