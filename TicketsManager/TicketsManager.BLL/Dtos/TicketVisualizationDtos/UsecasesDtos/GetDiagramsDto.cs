using TicketsManager.Common.Dtos;

namespace TicketsManager.BLL.Dtos.TicketVisualizationDtos.UsecasesDtos
{
    public class GetDiagramsDto
    {
        public PaginationRequestDto PaginationRequest { get; set; }
        public Guid TicketId { get; set; }
    }
}
