using AutoMapper;
using TicketsManager.BLL.Dtos.TicketUsecaseDtos;
using TicketsManager.BLL.Dtos.TicketUsecaseDtos.FullDtos;
using TicketsManager.BLL.Dtos.TicketUsecaseDtos.UsecasesDtos;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Models;

namespace TicketsManager.BLL.Mapping;

public class TicketUsecaseProfile : Profile
{
    public TicketUsecaseProfile()
    {
        CreateMap<TicketDiagramDto, TicketDiagram>()
            .ForMember(dest => dest.Id, opts =>
                opts.MapFrom(new GuidValueResolver()))
            .ForMember(dest => dest.Usecase, opts =>
                opts.Ignore())
            .ForMember(dest => dest.UsecaseId, opts =>
                opts.Ignore())
            .ReverseMap();

        CreateMap<TicketTableDto, TicketTable>()
            .ForMember(dest => dest.Id, opts =>
                opts.MapFrom(new GuidValueResolver()))
            .ForMember(dest => dest.Usecase, opts =>
                opts.Ignore())
            .ForMember(dest => dest.UsecaseId, opts =>
                opts.Ignore())
            .ReverseMap();

        CreateMap<UsecaseDto, Usecase>()
            .ForMember(dest => dest.Id, opts =>
                opts.MapFrom(new GuidValueResolver()))
            .ForMember(dest => dest.TicketId, opts =>
                opts.Ignore())
            .ForMember(dest => dest.Ticket, opts =>
                opts.Ignore());

        CreateMap<TicketDiagramFullDto, TicketDiagram>()
            .ForMember(dest => dest.Usecase, opts =>
                opts.Ignore())
            .ForMember(dest => dest.UsecaseId, opts =>
                opts.Ignore())
            .ReverseMap();

        CreateMap<TicketTableFullDto, TicketTable>()
            .ForMember(dest => dest.Usecase, opts =>
                opts.Ignore())
            .ForMember(dest => dest.UsecaseId, opts =>
                opts.Ignore())
            .ReverseMap();

        CreateMap<UsecaseFullDto, Usecase>()
            .ForMember(dest => dest.Ticket, opts =>
                opts.Ignore())
            .ForMember(dest => dest.TicketId, opts =>
                opts.Ignore())
            .ReverseMap();

        CreateMap<Usecase, UsecaseFullDto>()
            .ForMember(dest => dest.TicketSummaryIds, opts =>
                opts.MapFrom(_ => GetTicketSummaryIds(_.TicketSummaries)));

        CreateMap<CreateUsecasesDto, Usecase>()
            .ForMember(dest => dest.Id, opts =>
                opts.MapFrom(new GuidValueResolver()))
            .ForMember(dest => dest.Title, opts =>
                opts.MapFrom(_ => _.Usecase.Title))
            .ForMember(dest => dest.Tables, opts =>
                opts.MapFrom(_ => _.Usecase.Tables))
            .ForMember(dest => dest.Diagrams, opts =>
                opts.MapFrom(_ => _.Usecase.Diagrams))
            .ForMember(dest => dest.Description, opts =>
                opts.MapFrom(_ => _.Usecase.Description))
            .ForMember(dest => dest.Ticket, opts =>
                opts.Ignore())
            .ForMember(dest => dest.TicketId, opts =>
                opts.MapFrom(_ => _.TicketId))
            .ForMember(dest => dest.TicketSummaries, opts =>
                opts.MapFrom(_ => CreateTicketSummaryListFromIds(_.TicketSummaryIds)));

        CreateMap<UsecaseDataFullDto, Usecase>()
            .ForMember(dest => dest.Ticket, opts =>
                opts.Ignore())
            .ForMember(dest => dest.TicketId, opts =>
                opts.Ignore());

        CreateMap<PaginationResponseDto<Usecase>, PaginationResponseDto<UsecaseFullDto>>();
    }

    private static List<Guid> GetTicketSummaryIds(ICollection<TicketSummary>? ticketSummaries)
    {
        if (ticketSummaries is null)
            return Enumerable.Empty<Guid>().ToList();

        return ticketSummaries.Select(a => a.Id).ToList();
    }

    private static List<TicketSummary> CreateTicketSummaryListFromIds(List<Guid>? ticketSummaryIds)
    {
        if (ticketSummaryIds is null)
            return Enumerable.Empty<TicketSummary>().ToList();

        var ticketSummaries = new List<TicketSummary>();

        foreach (var ticketSummaryId in ticketSummaryIds)
            ticketSummaries.Add(new TicketSummary { Id = ticketSummaryId, Data = string.Empty });

        return ticketSummaries;
    }
}
