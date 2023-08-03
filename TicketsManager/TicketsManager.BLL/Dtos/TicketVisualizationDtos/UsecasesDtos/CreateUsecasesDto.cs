namespace TicketsManager.BLL.Dtos.TicketVisualizationDtos.UsecasesDtos
{
    public class CreateUsecasesDto
    {
        public TicketTableDto TicketTable { get; set; }
        public List<TicketDiagramDto> TicketDiagrams { get; set; }
        public Guid TicketId { get; set; }
    }
}
