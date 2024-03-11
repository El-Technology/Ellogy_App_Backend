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
    public async Task AddUserStoryTestAsync(List<CreateUserStoryTestDto> userStoryTest)
    {
        var mappedUserStoryTest = _mapper.Map<List<UserStoryTest>>(userStoryTest);
        await _userStoryTestRepository.AddUserStoryTestAsync(mappedUserStoryTest);
    }


    /// <inheritdoc cref="IUserStoryTestService.GetUserStoryTestsAsync" />
    public async Task<List<GetUserStoryDto>> GetUserStoryTestsAsync(Guid ticketId)
    {
        return _mapper.Map<List<GetUserStoryDto>>(await _userStoryTestRepository.GetUserStoryTests(ticketId)
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

    /// <inheritdoc cref="IUserStoryTestService.DeleteTestCasesByIds" />
    public async Task DeleteTestCasesByIds(List<Guid> listOfTestCaseIds)
    {
        await _userStoryTestRepository.DeleteTestCasesByIds(listOfTestCaseIds);
    }
}