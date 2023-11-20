using TicketsManager.BLL.Dtos.ActionHistoryDtos;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Models;

namespace TicketsManager.BLL.Interfaces
{
    public interface IActionHistoryService
    {
        Task CreateActionHistoryAsync(CreateActionHistoryDto createActionHistoryDto);
        Task<PaginationResponseDto<ActionHistory>> GetActionHistoriesAsync(Guid ticketId, SearchHistoryRequestDto searchHistoryRequestDto);
    }
}
