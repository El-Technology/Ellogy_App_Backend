#pragma warning disable CS8618
namespace TicketsManager.BLL.Dtos.MessageDtos;

public class MessageCreateRequestDto
{
    public Guid TicketId { get; set; }
    public string Sender { get; set; }
    public string Content { get; set; }
}
