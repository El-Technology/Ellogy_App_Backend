using AutoMapper;
using TicketsManager.BLL.Dtos.ActionHistoryDtos;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.BLL.Mapping;

public class ActionHistoryProfile : Profile
{
    public ActionHistoryProfile()
    {
        CreateMap<CreateActionHistoryDto, ActionHistory>()
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(new GuidValueResolver()))
            .ForMember(dest => dest.ActionTime, opt =>
                opt.MapFrom(_ => DateTime.UtcNow));
    }
}
