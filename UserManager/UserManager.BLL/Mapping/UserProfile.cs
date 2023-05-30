using AutoMapper;
using UserManager.BLL.Dtos;
using UserManager.BLL.Helpers;
using UserManager.DAL.Models;

namespace UserManager.BLL.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserRegisterRequestDto, User>()
                .ForMember(dest => dest.Id, opts =>
                    opts.MapFrom<GuidValueResolver>())
                .ForMember(dest => dest.Salt, opt =>
                    opt.MapFrom(_ => CryptoHelper.GenerateSalt()))
                .ForMember(dest => dest.Password, opts =>
                    opts.Ignore());

            CreateMap<User, UserRegisterResponseDto>();
        }
    }
}
