using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TicketsManager.BLL.Dtos.UserStoryTestDtos;
using TicketsManager.BLL.Dtos.UserStoryTestDtos.GetDtos;
using TicketsManager.BLL.Interfaces;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.UserStoryTestsModels;

namespace TicketsManager.BLL.Services;

public class UserStoryTestService : IUserStoryTestService
{
    private readonly IMapper _mapper;
    private readonly IUserStoryTestRepository _userStoryTestRepository;

    public UserStoryTestService(IUserStoryTestRepository userStoryTestRepository, IMapper mapper)
    {
        _mapper = mapper;
        _userStoryTestRepository = userStoryTestRepository;
    }

    /// <inheritdoc cref="IUserStoryTestService.AddUserStoryTestAsync" />
    public async Task<List<GetUserStoryDto>> AddUserStoryTestAsync(
        List<CreateUserStoryTestDto> userStoryTest)
    {
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
    public async Task<List<GetUserStoryDto>> GetUserStoryTestsAsync(Guid ticketId)
    {
        return _mapper.Map<List<GetUserStoryDto>>(
            await _userStoryTestRepository
                .GetUserStoryTests(ticketId)
                .ToListAsync());
    }

    /// <inheritdoc cref="IUserStoryTestService.UpdateUserStoryTestAsync" />
    public async Task UpdateUserStoryTestAsync(List<UpdateUserStoryTestDto> userStoryTest)
    {
        var mappedUserStoryTest = _mapper.Map<List<UserStoryTest>>(userStoryTest);
        await _userStoryTestRepository.UpdateUserStoryTestAsync(mappedUserStoryTest);
    }

    /// <inheritdoc cref="IUserStoryTestService.DeleteUserStoryTestAsync" />
    public async Task DeleteUserStoryTestAsync(Guid ticketId)
    {
        await _userStoryTestRepository.DeleteUserStoryTestByTicketIdAsync(ticketId);
    }

    /// <inheritdoc cref="IUserStoryTestService.DeleteTestCasesByIdsAsync" />
    public async Task DeleteTestCasesByIdsAsync(List<Guid> listOfTestCaseIds)
    {
        await _userStoryTestRepository.DeleteTestCasesByIds(listOfTestCaseIds);
    }
}