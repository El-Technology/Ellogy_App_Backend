using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Interfaces
{
    public interface IActionHistoryRepository
    {
        Task CreateActionHistoryAsync(ActionHistory actionHistory);
        Task<PaginationResponseDto<ActionHistory>> GetActionHistoriesAsync(Guid ticketId, TicketCurrentStepEnum ticketCurrentStepEnum, PaginationRequestDto paginationRequest);
    }
}
