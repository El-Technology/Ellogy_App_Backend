using AICommunicationService.BLL.Dtos;
using AICommunicationService.Common.Dtos;
using AICommunicationService.DAL.Models;
using AutoMapper;

namespace AICommunicationService.BLL.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();

        CreateMap<PaginationResponseDto<User>, PaginationResponseDto<UserDto>>();
    }
}