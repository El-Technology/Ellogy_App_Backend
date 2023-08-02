namespace TicketsManager.BLL.Dtos.TicketVisualizationDtos.FullDtos
{
    public class TicketTableFullDto
    {
        public Guid Id { get; set; }
        public string TableKey { get; set; }
        public List<TicketTableValueFullDto> TableValues { get; set; }
    }
}
