namespace TicketsManager.BLL.Dtos.TicketUsecaseDtos.FullDtos;

public class UsecaseFullDto : UsecaseDataFullDto
{
    public Guid Id { get; set; }
    public List<Guid>? TicketSummaryIds { get; set; }
}
