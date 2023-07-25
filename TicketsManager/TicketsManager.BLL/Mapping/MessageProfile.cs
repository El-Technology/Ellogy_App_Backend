using AutoMapper;
using TicketsManager.BLL.Dtos.MessageDtos;
using TicketsManager.DAL.Models;

namespace TicketsManager.BLL.Mapping;

public class MessageProfile : Profile
{
    public MessageProfile()
    {
        CreateMap<Message, MessageResponseDto>()
            .ForMember(dest => dest.Action, opts =>
            opts.MapFrom(_ => new ActionDto { State = _.ActionState, Type = _.ActionType }));

        CreateMap<MessageDto, Message>()
            .ForMember(dest => dest.ActionState, opts =>
                opts.MapFrom(_ => _.Action != null ? _.Action.State : null))
            .ForMember(dest => dest.ActionType, opts =>
                opts.MapFrom(_ => _.Action != null ? _.Action.Type : null));
    }
}
