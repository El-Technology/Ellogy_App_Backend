using AutoMapper;
using TicketsManager.BLL.Dtos.NotificationDtos;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.BLL.Mapping
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<NotificationDto, Notification>()
                .ForMember(dest => dest.Id, opts =>
                    opts.MapFrom(new GuidValueResolver()))
                .ForMember(dest => dest.TicketId, opts =>
                    opts.Ignore())
                .ForMember(dest => dest.Ticket, opts =>
                    opts.Ignore());

            CreateMap<Notification, NotificationFullDto>()
                .ReverseMap();
        }
    }
}
