using TicketsManager.BLL.Dtos.TicketVisualizationDtos.FullDtos;

namespace TicketsManager.BLL.Dtos.TicketVisualizationDtos.UpdateDtos
{
    public class TicketTableUpdateDto
    {
        public string TableKey { get; set; }
        public List<TicketTableValueFullDto> TableValues { get; set; }

    }
}
