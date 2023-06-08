using TicketsManager.BLL.Dtos.MessageDtos;

namespace TicketsManager.BLL.Interfaces;

public interface IMessageService
{
    Task<MessageResponseDto> CreateMessageAsync(MessageCreateRequestDto messageCreateRequest);
}
