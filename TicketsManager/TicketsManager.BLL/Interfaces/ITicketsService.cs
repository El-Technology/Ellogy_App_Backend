using TicketsManager.BLL.Dtos.TicketDtos;
using TicketsManager.Common.Helpers.Pagination;

namespace TicketsManager.BLL.Interfaces;

public interface ITicketsService
{
    Task<PaginationResponseDto<TicketResponseDto>> GetTicketsAsync(Guid userId, PaginationRequestDto paginateRequest);
    Task<TicketResponseDto> CreateTicketAsync(TicketCreateRequestDto createTicketRequest, Guid userId);
    Task DeleteTicketAsync(Guid id);
    Task<TicketResponseDto> UpdateTicketAsync(TicketUpdateRequestDto ticketUpdate);
}
