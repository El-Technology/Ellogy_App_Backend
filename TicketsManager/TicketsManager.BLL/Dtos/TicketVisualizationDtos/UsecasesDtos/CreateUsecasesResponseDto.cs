using TicketsManager.BLL.Dtos.TicketVisualizationDtos.FullDtos;

namespace TicketsManager.BLL.Dtos.TicketVisualizationDtos.UsecasesDtos
{
    public class CreateUsecasesResponseDto
    {
        public TicketTableFullDto TicketTable { get; set; }
        public List<TicketDiagramFullDto> TicketDiagrams { get; set; }
    }
}
