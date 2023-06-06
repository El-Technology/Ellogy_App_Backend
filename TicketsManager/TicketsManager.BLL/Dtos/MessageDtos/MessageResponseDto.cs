namespace TicketsManager.BLL.Dtos.MessageDtos;

public class MessageResponseDto
{
    public Guid Id { get; set; }
    public string Sender { get; set; }
    public string Content { get; set; }
}
