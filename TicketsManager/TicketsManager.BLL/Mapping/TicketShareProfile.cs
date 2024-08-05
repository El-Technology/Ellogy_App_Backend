using AutoMapper;
using TicketsManager.BLL.Dtos.TicketShareDtos;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.BLL.Mapping;
public class TicketShareProfile : Profile
{
    public TicketShareProfile()
    {
        CreateMap<TicketShare, PermissionDto>();

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

        CreateMap<CreateTicketShareBaseDto, TicketShare>()
                   .ForMember(dest => dest.Id, opt => opt.Ignore())
                   .ForMember(dest => dest.GivenAt, opt => opt.Ignore())
                   .ForMember(dest => dest.Ticket, opt => opt.Ignore())
                   .ForMember(dest => dest.TicketId, opt => opt.Ignore())
                   .ForMember(dest => dest.SharedUserId, opt => opt.Ignore())
                   .ForMember(dest => dest.RevokedAt, opt => opt.Ignore())
                   .ForMember(dest => dest.Permission, opt => opt.Ignore());

        CreateMap<CreateManyTicketShareDto, TicketShare>()
            .ForMember(dest => dest.TicketCurrentStep, opt => opt.Ignore())
            .ForMember(dest => dest.SubStageEnum, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.GivenAt, opt => opt.Ignore())
            .ForMember(dest => dest.Ticket, opt => opt.Ignore());

        CreateMap<PaginationResponseDto<TicketShare>, PaginationResponseDto<GetTicketShareDto>>();
    }
}
