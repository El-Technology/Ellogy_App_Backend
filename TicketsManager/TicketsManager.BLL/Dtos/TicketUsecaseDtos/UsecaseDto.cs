namespace TicketsManager.BLL.Dtos.TicketUsecaseDtos
{
    public class UsecaseDto
    {
        public string Title { get; set; }
        public List<TicketTableDto> Tables { get; set; }
        public List<TicketDiagramDto> Diagrams { get; set; }
    }
}
