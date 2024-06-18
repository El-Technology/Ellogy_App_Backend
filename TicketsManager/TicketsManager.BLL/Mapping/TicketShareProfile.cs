using AutoMapper;
using TicketsManager.BLL.Dtos.TicketShareDtos;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.BLL.Mapping;
public class TicketShareProfile : Profile
{
    public TicketShareProfile()
    {
        CreateMap<CreateTicketShareDto, TicketShare>()
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(new GuidValueResolver()))
            .ForMember(dest => dest.GivenAt, opt =>
                opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<TicketShare, GetTicketShareDto>()
            .ForPath(dest => dest.UserDto.Id, opt =>
                opt.MapFrom(src => src.SharedUserId));

        CreateMap<UpdateTicketShareDto, TicketShare>()
            .ForMember(dest => dest.Ticket, opt =>
                opt.Ignore());

        CreateMap<PaginationResponseDto<TicketShare>, PaginationResponseDto<GetTicketShareDto>>();
    }
}
