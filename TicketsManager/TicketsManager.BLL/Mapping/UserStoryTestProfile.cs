using AutoMapper;
using TicketsManager.BLL.Dtos.UserStoryTestDtos;
using TicketsManager.BLL.Dtos.UserStoryTestDtos.GetDtos;
using TicketsManager.DAL.Dtos;
using TicketsManager.DAL.Models.UserStoryTests;

namespace TicketsManager.BLL.Mapping;

public class UserStoryTestProfile : Profile
{
    public UserStoryTestProfile()
    {
        CreateMap<CreateUserStoryTestDto, UserStoryTest>()
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(new GuidValueResolver()))
            .ForMember(dest => dest.TicketSummary, opt =>
                opt.Ignore());

        CreateMap<CreateTestCaseDto, TestCase>()
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(new GuidValueResolver()))
            .ForMember(dest => dest.UserStoryTest, opt =>
                opt.Ignore())
            .ForMember(dest => dest.UserStoryTestId, opt =>
                opt.Ignore());

        CreateMap<CreateTestPlanDto, TestPlan>()
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(new GuidValueResolver()))
            .ForMember(dest => dest.UserStoryTest, opt =>
                opt.Ignore())
            .ForMember(dest => dest.UserStoryTestId, opt =>
                opt.Ignore());

        CreateMap<ReturnUserStoryTestModel, GetUserStoryDto>()
            .ForMember(dest => dest.TicketSummaryData, opt =>
                opt.MapFrom(t => t.TicketSummaryData));

        CreateMap<UserStoryTest, GetUserStoryDto>()
            .ForMember(dest => dest.TicketSummaryData, opt =>
                opt.MapFrom(t => t.TicketSummary!.Data));

        CreateMap<GetUserStoryDto, UserStoryTest>()
            .ForMember(dest => dest.TicketSummary, opt =>
                opt.Ignore());

        CreateMap<TestCase, GetTestCaseDto>()
            .ReverseMap();
        CreateMap<TestPlan, GetTestPlanDto>()
            .ReverseMap();

        CreateMap<UpdateUserStoryTestDto, UserStoryTest>()
            .ForMember(dest => dest.TicketSummary, opt =>
                opt.Ignore());
    }
}