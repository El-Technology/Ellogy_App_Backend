using AutoMapper;
using UserManager.BLL.Dtos.LoginDtos;
using UserManager.BLL.Dtos.RegisterDtos;
using UserManager.Common.Helpers;
using UserManager.DAL.Enums;
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
                .ForMember(dest => dest.Email, opts =>
                    opts.MapFrom(src => src.Email.Trim().ToLower()))
                .ForMember(dest => dest.Salt, opt =>
                    opt.MapFrom(_ => CryptoHelper.GenerateSalt()))
                .ForMember(dest => dest.Password, opts =>
                    opts.Ignore())
                .ForMember(dest => dest.Role, opts =>
                    opts.MapFrom(_ => RoleEnum.User));

            CreateMap<User, LoginResponseDto>()
                .ForMember(dest => dest.Email, opts =>
                    opts.MapFrom(src => src.Email.Trim().ToLower()));
        }
    }
}
