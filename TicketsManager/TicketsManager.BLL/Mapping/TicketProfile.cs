using AutoMapper;
using TicketsManager.BLL.Dtos.TicketDtos;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Models;

namespace TicketsManager.BLL.Mapping;

public class TicketProfile : Profile
{
    public TicketProfile()
    {
        CreateMap<TicketCreateRequestDto, Ticket>()
            .ForMember(dest => dest.Id, opts =>
                opts.MapFrom(new GuidValueResolver()))
            .ForMember(dest => dest.TicketMessages, opts =>
                opts.MapFrom(_ => new List<Message>()))
            .ForMember(dest => dest.Summary, opts =>
                opts.Ignore())
            .ForMember(dest => dest.Comment, opts =>
                opts.Ignore())
            .ForMember(dest => dest.UpdatedDate, opts =>
                opts.Ignore())
            .ForMember(dest => dest.User, opts =>
                opts.Ignore())
            .ForMember(dest => dest.UserId, opts =>
                opts.Ignore());

        CreateMap<Ticket, TicketResponseDto>();

        CreateMap<TicketUpdateRequestDto, Ticket>()
            .ForMember(dest => dest.TicketMessages, opts =>
                opts.Ignore())
            .ForMember(dest => dest.CreatedDate, opts =>
                opts.Ignore())
            .ForMember(dest => dest.User, opts =>
                opts.Ignore())
            .ForMember(dest => dest.UserId, opts =>
                opts.Ignore());

        CreateMap<PaginationResponseDto<Ticket>, PaginationResponseDto<TicketResponseDto>>();
    }
}
