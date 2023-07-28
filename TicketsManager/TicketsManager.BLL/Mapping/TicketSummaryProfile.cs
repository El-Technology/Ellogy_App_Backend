using AutoMapper;
using TicketsManager.BLL.Dtos.TicketSummaryDtos;
using TicketsManager.DAL.Models;

namespace TicketsManager.BLL.Mapping
{
    public class TicketSummaryProfile : Profile
    {
        public TicketSummaryProfile()
        {
            CreateMap<TicketSummaryRequestDto, TicketSummary>()
                .ForMember(dest => dest.Id, opt =>
                    opt.MapFrom(new GuidValueResolver()))
                .ForMember(dest => dest.Ticket, opt =>
                    opt.Ignore())
                .ForMember(dest => dest.TicketId, opt =>
                    opt.Ignore());

            CreateMap<TicketSummaryFullDto, TicketSummary>()
                .ForMember(dest => dest.Ticket, opt =>
                    opt.Ignore())
                .ForMember(dest => dest.TicketId, opt =>
                    opt.Ignore())
                .ReverseMap();
        }
    }
}
