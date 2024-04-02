using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TicketsManager.BLL.Dtos.UserStoryTestDtos;
using TicketsManager.BLL.Dtos.UserStoryTestDtos.GetDtos;
using TicketsManager.BLL.Interfaces;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.UserStoryTests;

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
        var mappedUserStoryTest = _mapper.Map<List<UserStoryTest>>(userStoryTest);

        if (await _userStoryTestRepository.GetUserStoryTests(mappedUserStoryTest).AnyAsync())
            throw new Exception("User story test already exists");

        await _userStoryTestRepository.AddUserStoryTestAsync(mappedUserStoryTest);

        return _mapper.Map<List<GetUserStoryDto>>(
            await _userStoryTestRepository
                .GetUserStoryTests(mappedUserStoryTest)
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