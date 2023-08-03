using AutoMapper;
using TicketsManager.BLL.Dtos.TicketVisualizationDtos;
using TicketsManager.BLL.Dtos.TicketVisualizationDtos.FullDtos;
using TicketsManager.BLL.Dtos.TicketVisualizationDtos.UpdateDtos;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Models;

namespace TicketsManager.BLL.Mapping
{
    public class TicketVisualizationProfile : Profile
    {
        public TicketVisualizationProfile()
        {
            CreateMap<TicketDiagramDto, TicketDiagram>()
                .ForMember(dest => dest.Id, opts =>
                    opts.MapFrom(new GuidValueResolver()))
                .ForMember(dest => dest.Ticket, opts =>
                    opts.Ignore())
                .ForMember(dest => dest.TicketId, opts =>
                    opts.Ignore())
                .ReverseMap();

            CreateMap<TicketTableDto, TicketTable>()
                .ForMember(dest => dest.Id, opts =>
                    opts.MapFrom(new GuidValueResolver()))
                .ForMember(dest => dest.Ticket, opts =>
                    opts.Ignore())
                .ForMember(dest => dest.TicketId, opts =>
                    opts.Ignore())
                .ReverseMap();

            CreateMap<TicketTableValueDto, TicketTableValue>()
                .ForMember(dest => dest.Id, opts =>
                    opts.MapFrom(new GuidValueResolver()))
                .ForMember(dest => dest.TicketTable, opts =>
                    opts.Ignore())
                .ForMember(dest => dest.TicketTableId, opts =>
                    opts.Ignore())
                .ReverseMap();

            CreateMap<TicketDiagramFullDto, TicketDiagram>()
                .ForMember(dest => dest.Ticket, opts =>
                    opts.Ignore())
                .ForMember(dest => dest.TicketId, opts =>
                    opts.Ignore())
                .ReverseMap();

            CreateMap<TicketTableFullDto, TicketTable>()
                .ForMember(dest => dest.Ticket, opts =>
                    opts.Ignore())
                .ForMember(dest => dest.TicketId, opts =>
                    opts.Ignore())
                .ReverseMap();

            CreateMap<TicketTableValueFullDto, TicketTableValue>()
                .ForMember(dest => dest.TicketTable, opts =>
                    opts.Ignore())
                .ForMember(dest => dest.TicketTableId, opts =>
                    opts.Ignore())
                .ReverseMap();

            CreateMap<TicketTableUpdateDto, TicketTable>()
                .ForMember(dest => dest.TicketId, opts =>
                    opts.Ignore())
                .ForMember(dest => dest.Ticket, opts =>
                    opts.Ignore());

            CreateMap<TicketDiagramUpdateDto, TicketDiagram>()
                .ForMember(dest => dest.TicketId, opts =>
                    opts.Ignore())
                .ForMember(dest => dest.Ticket, opts =>
                    opts.Ignore());

            CreateMap<PaginationResponseDto<TicketDiagram>, PaginationResponseDto<TicketDiagramFullDto>>();
        }
    }
}
