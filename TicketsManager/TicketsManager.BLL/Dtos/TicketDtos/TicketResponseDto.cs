using TicketsManager.BLL.Dtos.MessageDtos;
using TicketsManager.DAL.Enums;

namespace TicketsManager.BLL.Dtos.TicketDtos;

public class TicketResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string? Summary { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public TicketStatusEnum Status { get; set; }

    public ICollection<MessageResponseDto> TicketMessages { get; set; } = new List<MessageResponseDto>();
}
