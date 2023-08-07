using TicketsManager.Common.Dtos;

namespace TicketsManager.BLL.Dtos.TicketUsecaseDtos.UsecasesDtos
{
    public class GetUsecasesDto
    {
        public PaginationRequestDto PaginationRequest { get; set; }
        public Guid TicketId { get; set; }
    }
}
