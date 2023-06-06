using TicketsManager.DAL.Enums;

namespace TicketsManager.BLL.Dtos.TicketDtos;

public class TicketUpdateRequestDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string? Summary { get; set; }
    public string? Comment { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public TicketStatusEnum Status { get; set; }
}
