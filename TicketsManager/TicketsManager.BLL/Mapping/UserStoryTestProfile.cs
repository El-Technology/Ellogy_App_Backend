using AutoMapper;
using System;
using System.Linq;
using TicketsManager.BLL.Dtos.UserStoryTestDtos;
using TicketsManager.BLL.Dtos.UserStoryTestDtos.GetDtos;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Dtos;
using TicketsManager.DAL.Models.UserStoryTestsModels;

namespace TicketsManager.BLL.Mapping;

public class UserStoryTestProfile : Profile
{
    public UserStoryTestProfile()
    {
        CreateMap<CreateUserStoryTestDto, UserStoryTest>()
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(new GuidValueResolver()))
            .ForMember(dest => dest.RelatedSummaries, opt =>
                opt.MapFrom(src => src.RelatedSummaryIds.Select(id => new UserStoryTestTicketSummary
                {
                    Id = Guid.NewGuid(),
                    TicketSummaryId = id
                })));

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
            .ForMember(dest => dest.RelatedSummaryIds, opt =>
                opt.MapFrom(src => src.RelatedSummaries.Select(rs => rs.TicketSummaryId)));

        CreateMap<UserStoryTest, GetUserStoryDto>()
            .ForMember(dest => dest.RelatedSummaryIds, opt =>
                opt.MapFrom(src => src.RelatedSummaries.Select(rs => rs.TicketSummaryId)));

        CreateMap<GetUserStoryDto, UserStoryTest>()
            .ForMember(dest => dest.RelatedSummaries, opt => opt.Ignore());

        CreateMap<TestCase, GetTestCaseDto>()
            .ReverseMap();
        CreateMap<TestPlan, GetTestPlanDto>()
            .ReverseMap();

        CreateMap<UpdateUserStoryTestDto, UserStoryTest>()
            .ForMember(dest => dest.RelatedSummaries, opt => opt.Ignore());

        CreateMap<PaginationResponseDto<ReturnUserStoryTestModel>, PaginationResponseDto<GetUserStoryDto>>();
    }
}
