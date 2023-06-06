using AutoMapper;
using TicketsManager.BLL.Dtos.TicketDtos;
using TicketsManager.DAL.Models;

namespace TicketsManager.BLL.Mapping;

public class TicketProfile : Profile
{
    public TicketProfile()
    {
        CreateMap<TicketCreateRequestDto, Ticket>()
            .ForMember(dest => dest.Id, opts =>
                opts.MapFrom(new GuidValueResolver()))
            .ForMember(dest => dest.Summary, opts =>
                opts.Ignore())
            .ForMember(dest => dest.Comment, opts =>
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
    }
}
