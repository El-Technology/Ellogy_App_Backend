namespace TicketsManager.BLL.Dtos.TicketUsecaseDtos.FullDtos
{
    public class UsecaseDataFullDto
    {
        public string Title { get; set; }
        public List<TicketDiagramFullDto> Diagrams { get; set; }
        public List<TicketTableFullDto> Tables { get; set; }
    }
}

