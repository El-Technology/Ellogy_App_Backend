using AutoMapper;
using TicketsManager.BLL.Dtos.MessageDtos;
using TicketsManager.BLL.Exceptions;
using TicketsManager.BLL.Interfaces;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models;

namespace TicketsManager.BLL.Services;

public class MessageService : IMessageService
{
    private readonly IMapper _mapper;
    private readonly IMessagesRepository _messagesRepository;
    private readonly ITicketsRepository _ticketsRepository;

    public MessageService(IMapper mapper, IMessagesRepository messagesRepository, ITicketsRepository ticketsRepository)
    {
        _mapper = mapper;
        _messagesRepository = messagesRepository;
        _ticketsRepository = ticketsRepository;
    }

    public async Task<MessageResponseDto> CreateMessageAsync(MessageCreateRequestDto messageCreateRequest)
    {
        if (!await _ticketsRepository.CheckIfTicketExistAsync(messageCreateRequest.TicketId))
            throw new TicketNotFoundException(messageCreateRequest.TicketId);

        var mappedMessage = _mapper.Map<Message>(messageCreateRequest);
        await _messagesRepository.CreateMessageAsync(mappedMessage);

        return _mapper.Map<MessageResponseDto>(mappedMessage);
    }
}
