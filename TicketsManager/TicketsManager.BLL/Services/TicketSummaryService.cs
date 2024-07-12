using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TicketsManager.BLL.Dtos.TicketSummaryDtos;
using TicketsManager.BLL.Interfaces;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.TicketSummaryModels;

namespace TicketsManager.BLL.Services;

public class TicketSummaryService : ITicketSummaryService
{
    private readonly IMapper _mapper;
    private readonly ITicketSummaryRepository _ticketSummaryRepository;
    private readonly ITicketShareRepository _ticketShareRepository;

    public TicketSummaryService(
        ITicketSummaryRepository ticketSummaryRepository,
        IMapper mapper,
        ITicketShareRepository ticketShareRepository)
    {
        _mapper = mapper;
        _ticketSummaryRepository = ticketSummaryRepository;
        _ticketShareRepository = ticketShareRepository;
    }

    #region Private methods

    private async Task ValidateUserPermissionAsync(
    Guid ticketId, Guid userIdFromToken, SharePermissionEnum sharePermissionEnum)
    {
        await _ticketShareRepository.CheckIfUserHaveAccessToComponentStrictAsync(
            ticketId,
            userIdFromToken,
            TicketCurrentStepEnum.General,
            sharePermissionEnum);
    }

    #endregion

    /// <inheritdoc cref="ITicketSummaryService.GetTicketSummariesByTicketIdAsync" />
    public async Task<List<TicketSummaryFullDto>> GetTicketSummariesByTicketIdAsync(Guid userId, Guid ticketId)
    {
        await ValidateUserPermissionAsync(ticketId, userId, SharePermissionEnum.Read);

        return _mapper.Map<List<TicketSummaryFullDto>>(await _ticketSummaryRepository
            .GetTicketSummariesByTicketIdAsync(ticketId).ToListAsync());
    }

    /// <inheritdoc cref="ITicketSummaryService.GetTicketSummariesByTicketIdAsync" />
    public async Task<PaginationResponseDto<TicketSummaryFullDto>> GetTicketSummariesByTicketIdAsync(
        Guid userId, Guid ticketId, PaginationRequestDto paginationRequestDto)
    {
        await ValidateUserPermissionAsync(ticketId, userId, SharePermissionEnum.Read);

        return _mapper.Map<PaginationResponseDto<TicketSummaryFullDto>>(await _ticketSummaryRepository
            .GetTicketSummariesAsync(ticketId, paginationRequestDto));
    }

    /// <inheritdoc cref="ITicketSummaryService.CreateTicketSummariesAsync" />
    public async Task<List<TicketSummaryFullDto>> CreateTicketSummariesAsync(
        Guid userId, List<TicketSummaryCreateDto> ticketSummaries)
    {
        await ValidateUserPermissionAsync(
            ticketSummaries.FirstOrDefault()!.TicketId,
            userId,
            SharePermissionEnum.ReadWrite);

        var mappedTicketSummaries = _mapper.Map<List<TicketSummary>>(ticketSummaries);
        await _ticketSummaryRepository.CreateTicketSummariesAsync(mappedTicketSummaries);

        return _mapper.Map<List<TicketSummaryFullDto>>(mappedTicketSummaries);
    }

    /// <inheritdoc cref="ITicketSummaryService.UpdateTicketSummariesAsync" />
    public async Task<List<TicketSummaryFullDto>> UpdateTicketSummariesAsync(
        Guid userId, List<TicketSummaryFullDto> ticketSummaries)
    {
        await ValidateUserPermissionAsync(
            ticketSummaries.FirstOrDefault()!.TicketId,
            userId,
            SharePermissionEnum.ReadWrite);

        var mappedTicketSummaries = _mapper.Map<List<TicketSummary>>(ticketSummaries);
        await _ticketSummaryRepository.UpdateTicketSummariesAsync(mappedTicketSummaries);

        return _mapper.Map<List<TicketSummaryFullDto>>(mappedTicketSummaries);
    }

    /// <inheritdoc cref="ITicketSummaryService.DeleteTicketSummariesAsync" />
    public async Task DeleteTicketSummariesAsync(Guid userId, Guid ticketId)
    {
        await ValidateUserPermissionAsync(
           ticketId,
           userId,
           SharePermissionEnum.Manage);

        await _ticketSummaryRepository.DeleteTicketSummariesAsync(ticketId);
    }

    /// <inheritdoc cref="ITicketSummaryService.DeleteTicketSummaryScenariosAsync" />
    public async Task DeleteTicketSummaryScenariosAsync(Guid userId, Guid ticketId, List<Guid> summaryScenarioIds)
    {
        await ValidateUserPermissionAsync(
            ticketId,
            userId,
            SharePermissionEnum.Manage);

        await _ticketSummaryRepository.DeleteTicketSummaryScenariosAsync(summaryScenarioIds);
    }

    /// <inheritdoc cref="ITicketSummaryService.DeleteTicketSummaryAcceptanceCriteriaAsync" />
    public async Task DeleteTicketSummaryAcceptanceCriteriaAsync(Guid userId, Guid ticketId, List<Guid> summaryAcceptanceCriteriaIds)
    {
        await ValidateUserPermissionAsync(
            ticketId,
            userId,
            SharePermissionEnum.Manage);

        await _ticketSummaryRepository.DeleteTicketSummaryAcceptanceCriteriaAsync(summaryAcceptanceCriteriaIds);
    }

    /// <inheritdoc cref="ITicketSummaryService.DeleteTicketSummariesByIdsAsync" />
    public async Task DeleteTicketSummariesByIdsAsync(Guid userId, Guid ticketId, List<Guid> summaryIds)
    {
        await ValidateUserPermissionAsync(
            ticketId,
            userId,
            SharePermissionEnum.Manage);

        await _ticketSummaryRepository.DeleteTicketSummariesByIdAsync(summaryIds);
    }
}