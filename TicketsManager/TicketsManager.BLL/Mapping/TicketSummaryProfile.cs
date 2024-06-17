using AutoMapper;
using TicketsManager.BLL.Dtos.SummaryAcceptanceCriteriaDtos;
using TicketsManager.BLL.Dtos.SummaryScenarioDtos;
using TicketsManager.BLL.Dtos.TicketSummaryDtos;
using TicketsManager.DAL.Models.TicketSummaryModels;

namespace TicketsManager.BLL.Mapping;

public class TicketSummaryProfile : Profile
{
    public TicketSummaryProfile()
    {
        CreateMap<TicketSummaryCreateDto, TicketSummary>()
            .ForMember(dest => dest.Id, opts =>
                opts.MapFrom(new GuidValueResolver()))
            .ForMember(dest => dest.Ticket, opt =>
                opt.Ignore());

        CreateMap<SummaryScenarioCreateDto, SummaryScenario>()
            .ForMember(dest => dest.Id, opts =>
                opts.MapFrom(new GuidValueResolver()))
            .ForMember(dest => dest.TicketSummaryId, opt =>
                opt.Ignore())
            .ForMember(dest => dest.TicketSummary, opt =>
                opt.Ignore());

        CreateMap<SummaryAcceptanceCriteriaCreateDto, SummaryAcceptanceCriteria>()
            .ForMember(dest => dest.Id, opts =>
                opts.MapFrom(new GuidValueResolver()))
            .ForMember(dest => dest.TicketSummaryId, opt =>
                opt.Ignore())
            .ForMember(dest => dest.TicketSummary, opt =>
                opt.Ignore());

        CreateMap<TicketSummaryFullDto, TicketSummary>()
            .ForMember(dest => dest.Ticket, opt =>
                opt.Ignore())
            .ReverseMap();

        CreateMap<SummaryAcceptanceCriteriaDto, SummaryAcceptanceCriteria>()
            .ForMember(dest => dest.TicketSummaryId, opt =>
                opt.Ignore())
            .ForMember(dest => dest.TicketSummary, opt =>
                opt.Ignore())
            .ReverseMap();

        CreateMap<SummaryScenarioDto, SummaryScenario>()
            .ForMember(dest => dest.TicketSummaryId, opt =>
                opt.Ignore())
            .ForMember(dest => dest.TicketSummary, opt =>
                opt.Ignore())
            .ReverseMap();
    }
}