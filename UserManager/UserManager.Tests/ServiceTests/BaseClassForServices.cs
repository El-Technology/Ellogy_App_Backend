using AutoFixture;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserManager.BLL.Dtos.LoginDtos;
using UserManager.BLL.Dtos.ProfileDto;
using UserManager.BLL.Dtos.RegisterDtos;
using UserManager.BLL.Mapping;
using UserManager.Common.Helpers;
using UserManager.DAL.Context;
using UserManager.DAL.Enums;
using UserManager.DAL.Models;

namespace UserManager.Tests.ServiceTests
{
    public abstract class BaseClassForServices
    {
        protected IMapper _mapper;
        protected Fixture _fixture;
        protected UserManagerDbContext _userManagerDbContext;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<UserManagerDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDb")
                .Options;

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserRegisterRequestDto, User>()
                .ForMember(dest => dest.Id, opts =>
                    opts.MapFrom<GuidValueResolver>())
                .ForMember(dest => dest.Salt, opts =>
                    opts.MapFrom(_ => CryptoHelper.GenerateSalt()))
                .ForMember(dest => dest.Password, opts =>
                    opts.Ignore())
                .ForMember(dest => dest.AvatarLink, opts =>
                    opts.Ignore())
                .ForMember(dest => dest.Role, opts =>
                    opts.MapFrom(_ => RoleEnum.User));

                cfg.CreateMap<User, LoginResponseDto>()
                .ForMember(dest => dest.RefreshToken, opt =>
                    opt.Ignore());

                cfg.CreateMap<User, UserProfileDto>()
                .ReverseMap();
            });

            _mapper = mapperConfig.CreateMapper();

            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _userManagerDbContext = new UserManagerDbContext(options);
        }

        [TearDown]
        public async Task TearDown()
        {
            await _userManagerDbContext.DisposeAsync();
        }
    }
}
