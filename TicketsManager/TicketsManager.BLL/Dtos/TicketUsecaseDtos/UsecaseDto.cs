namespace TicketsManager.BLL.Dtos.TicketUsecaseDtos
{
    public class UsecaseDto
    {
        public List<TicketTableDto> Tables { get; set; }
        public List<TicketDiagramDto> Diagrams { get; set; }
    }
}
