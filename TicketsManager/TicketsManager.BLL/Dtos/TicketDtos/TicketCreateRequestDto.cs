using TicketsManager.BLL.Dtos.MessageDtos;
using TicketsManager.DAL.Enums;

namespace TicketsManager.BLL.Dtos.TicketDtos;

public class TicketCreateRequestDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public TicketStatusEnum Status { get; set; }
    public List<MessageCreateRequestDto> TicketMessages { get; set; }
}
