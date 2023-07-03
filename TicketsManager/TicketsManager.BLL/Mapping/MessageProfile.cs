using AutoMapper;
using TicketsManager.BLL.Dtos.MessageDtos;
using TicketsManager.DAL.Models;

namespace TicketsManager.BLL.Mapping;

public class MessageProfile : Profile
{
    public MessageProfile()
    {
        CreateMap<MessagesCreateRequestDto, Message>()
            .ForMember(dest => dest.Id, opts =>
                opts.MapFrom(new GuidValueResolver()))
            .ForMember(dest => dest.Ticket, opts =>
                opts.Ignore());

        CreateMap<Message, MessageResponseDto>();
        
        CreateMap<MessageDto, Message>();
    }
}
