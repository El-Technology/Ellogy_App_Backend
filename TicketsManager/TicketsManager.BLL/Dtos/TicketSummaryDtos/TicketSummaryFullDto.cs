namespace TicketsManager.BLL.Dtos.TicketSummaryDtos;

public class TicketSummaryFullDto : TicketSummaryRequestDto
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
}