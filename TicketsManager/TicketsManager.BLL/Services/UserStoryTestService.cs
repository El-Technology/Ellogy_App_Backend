using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TicketsManager.BLL.Dtos.UserStoryTestDtos;
using TicketsManager.BLL.Dtos.UserStoryTestDtos.GetDtos;
using TicketsManager.BLL.Interfaces;
using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.UserStoryTestsModels;

namespace TicketsManager.BLL.Services;

public class UserStoryTestService : IUserStoryTestService
{
    private readonly IMapper _mapper;
    private readonly IUserStoryTestRepository _userStoryTestRepository;
    private readonly ITicketShareRepository _ticketShareRepository;

    public UserStoryTestService(
        IUserStoryTestRepository userStoryTestRepository,
        IMapper mapper,
        ITicketShareRepository ticketShareRepository)
    {
        _mapper = mapper;
        _userStoryTestRepository = userStoryTestRepository;
        _ticketShareRepository = ticketShareRepository;
    }

    /// <inheritdoc cref="IUserStoryTestService.AddUserStoryTestAsync" />
    public async Task<List<GetUserStoryDto>> AddUserStoryTestAsync(
        Guid userId, List<CreateUserStoryTestDto> userStoryTest)
    {
        var ticketIdByUsecaseId = await _userStoryTestRepository.GetTicketIdByUsecaseIdAsync(
            userStoryTest.FirstOrDefault()!.UsecaseId ?? Guid.Empty);

        await _ticketShareRepository.CheckIfUserHaveAccessToComponentStrictAsync(
            ticketIdByUsecaseId,
            userId,
            TicketCurrentStepEnum.TestCases,
            SharePermissionEnum.ReadWrite);

        var mappedUserStoryTests = _mapper.Map<List<UserStoryTest>>(userStoryTest);

        if (await _userStoryTestRepository.GetUserStoryTests(mappedUserStoryTests).AnyAsync())
            throw new Exception("User story test already exists");

        var ticketIdsByUsecaseId = await _userStoryTestRepository
            .GetUsecaseTicketIdRelationAsync(mappedUserStoryTests.Select(u => u.UsecaseId ?? Guid.Empty).ToList());

        var lastOrders = new Dictionary<Guid, int>();

        foreach (var ticketId in ticketIdsByUsecaseId.Values.Distinct())
            lastOrders[ticketId] = await _userStoryTestRepository.GetLastOrderForStoryTestByTicketIdAsync(ticketId);

        foreach (var test in mappedUserStoryTests)
        {
            var ticketId = ticketIdsByUsecaseId[test.UsecaseId ?? Guid.Empty];

            if (lastOrders.ContainsKey(ticketId))
                test.Order = ++lastOrders[ticketId];
        }

        await _userStoryTestRepository.AddUserStoryTestAsync(mappedUserStoryTests);

        return _mapper.Map<List<GetUserStoryDto>>(
            await _userStoryTestRepository
                .GetUserStoryTests(mappedUserStoryTests)
                .ToListAsync());
    }


    /// <inheritdoc cref="IUserStoryTestService.GetUserStoryTestsAsync" />
    public async Task<List<GetUserStoryDto>> GetUserStoryTestsAsync(Guid userId, Guid ticketId)
    {
        await _ticketShareRepository.CheckIfUserHaveAccessToComponentStrictAsync(
            ticketId,
            userId,
            TicketCurrentStepEnum.TestCases,
            SharePermissionEnum.Read);

        return _mapper.Map<List<GetUserStoryDto>>(
            await _userStoryTestRepository
                .GetUserStoryTests(ticketId)
                .ToListAsync());
    }

    /// <inheritdoc cref="IUserStoryTestService.UpdateUserStoryTestAsync" />
    public async Task UpdateUserStoryTestAsync(Guid userId, List<UpdateUserStoryTestDto> userStoryTest)
    {
        var ticketIdByUsecaseId = await _userStoryTestRepository.GetTicketIdByUsecaseIdAsync(
            userStoryTest.FirstOrDefault()!.UsecaseId ?? Guid.Empty);

        await _ticketShareRepository.CheckIfUserHaveAccessToComponentStrictAsync(
            ticketIdByUsecaseId,
            userId,
            TicketCurrentStepEnum.TestCases,
            SharePermissionEnum.ReadWrite);

        var mappedUserStoryTest = _mapper.Map<List<UserStoryTest>>(userStoryTest);
        await _userStoryTestRepository.UpdateUserStoryTestAsync(mappedUserStoryTest);
    }

    /// <inheritdoc cref="IUserStoryTestService.DeleteUserStoryTestAsync" />
    public async Task DeleteUserStoryTestAsync(Guid userId, Guid ticketId)
    {
        await _ticketShareRepository.CheckIfUserHaveAccessToComponentStrictAsync(
            ticketId,
            userId,
            TicketCurrentStepEnum.TestCases,
            SharePermissionEnum.Manage);

        await _userStoryTestRepository.DeleteUserStoryTestByTicketIdAsync(ticketId);
    }

    /// <inheritdoc cref="IUserStoryTestService.DeleteTestCasesByIdsAsync" />
    public async Task DeleteTestCasesByIdsAsync(Guid userId, List<Guid> listOfTestCaseIds)
    {
        var ticketId = await _userStoryTestRepository
            .GetTicketIdByTestCaseIdAsync(listOfTestCaseIds.FirstOrDefault());

        await _ticketShareRepository.CheckIfUserHaveAccessToComponentStrictAsync(
            ticketId,
            userId,
            TicketCurrentStepEnum.TestCases,
            SharePermissionEnum.Manage);

        await _userStoryTestRepository.DeleteTestCasesByIds(listOfTestCaseIds);
    }
}