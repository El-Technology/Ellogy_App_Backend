using AutoMapper;
using TicketsManager.BLL.Dtos.TicketUsecaseDtos;
using TicketsManager.BLL.Dtos.TicketUsecaseDtos.FullDtos;
using TicketsManager.BLL.Dtos.TicketUsecaseDtos.UsecasesDtos;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Models;

namespace TicketsManager.BLL.Mapping
{
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

            CreateMap<CreateUsecasesDto, Usecase>()
                .ForMember(dest => dest.Id, opts =>
                    opts.MapFrom(new GuidValueResolver()))
                .ForMember(dest => dest.Title, opts =>
                    opts.MapFrom(_ => _.Usecase.Title))
                .ForMember(dest => dest.Tables, opts =>
                    opts.MapFrom(_ => _.Usecase.Tables))
                .ForMember(dest => dest.Diagrams, opts =>
                    opts.MapFrom(_ => _.Usecase.Diagrams))
                .ForMember(dest => dest.Ticket, opts =>
                    opts.Ignore())
                .ForMember(dest => dest.TicketId, opts =>
                    opts.MapFrom(_ => _.TicketId));

            CreateMap<UsecaseDataFullDto, Usecase>()
                .ForMember(dest => dest.Ticket, opts =>
                    opts.Ignore())
                .ForMember(dest => dest.TicketId, opts =>
                    opts.Ignore());

            CreateMap<PaginationResponseDto<Usecase>, PaginationResponseDto<UsecaseFullDto>>();
        }
    }
}
