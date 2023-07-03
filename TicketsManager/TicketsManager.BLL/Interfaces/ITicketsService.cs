using TicketsManager.BLL.Dtos.TicketDtos;
using TicketsManager.Common.Dtos;

namespace TicketsManager.BLL.Interfaces;

public interface ITicketsService
{
    Task<PaginationResponseDto<TicketResponseDto>> GetTicketsAsync(Guid userId, PaginationRequestDto paginateRequest);
    Task<TicketResponseDto> CreateTicketAsync(TicketCreateRequestDto createTicketRequest, Guid userId);
    Task DeleteTicketAsync(Guid id);
    Task<TicketResponseDto> UpdateTicketAsync(Guid id, TicketUpdateRequestDto ticketUpdate);
    Task<PaginationResponseDto<TicketResponseDto>> SearchTicketsByNameAsync(Guid userId, SearchTicketsRequestDto searchRequest);
}
