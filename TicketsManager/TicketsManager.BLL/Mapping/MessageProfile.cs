using AutoMapper;
using TicketsManager.BLL.Dtos.MessageDtos;
using TicketsManager.DAL.Models;

namespace TicketsManager.BLL.Mapping;

public class MessageProfile : Profile
{
    public MessageProfile()
    {
        CreateMap<Message, MessageResponseDto>();
        
        CreateMap<MessageDto, Message>();
    }
}
