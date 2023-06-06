using TicketsManager.BLL.Dtos.TicketDtos;

namespace TicketsManager.BLL.Interfaces;

public interface ITicketsService
{
    Task<ICollection<TicketResponseDto>> GetAllTicketsAsync(Guid userId);
    Task<TicketResponseDto> CreateTicketAsync(TicketCreateRequestDto createTicketRequest, Guid userId);
}
