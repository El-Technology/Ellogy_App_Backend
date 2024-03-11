using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TicketsManager.BLL.Dtos.TicketUsecaseDtos.FullDtos;
using TicketsManager.BLL.Dtos.TicketUsecaseDtos.UsecasesDtos;
using TicketsManager.BLL.Interfaces;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models;

namespace TicketsManager.BLL.Services;

public class UsecasesService : IUsecasesService
{
    private readonly ITicketSummaryRepository _ticketSummaryRepository;
    private readonly IUsecaseRepository _usecaseRepository;
    private readonly IMapper _mapper;
    public UsecasesService(IUsecaseRepository usecaseRepository, IMapper mapper, ITicketSummaryRepository ticketSummaryRepository)
    {
        _usecaseRepository = usecaseRepository;
        _mapper = mapper;
        _ticketSummaryRepository = ticketSummaryRepository;
    }

    private void ValidateUserPermission(Guid inputUserId, Guid userIdFromToken)
    {
        if (inputUserId != userIdFromToken)
            throw new Exception("You don't have permission to access another user data");
    }

    ///<inheritdoc cref="IUsecasesService.CreateUsecasesAsync(List{CreateUsecasesDto})"/>
    public async Task<CreateUsecasesResponseDto> CreateUsecasesAsync(List<CreateUsecasesDto> createUsecasesDto, Guid userIdFromToken)
    {
        foreach (var usecase in createUsecasesDto)
        {
            ValidateUserPermission(await _usecaseRepository.GetUserIdByTicketIdAsync(usecase.TicketId), userIdFromToken);
        }

        var usecases = _mapper.Map<List<Usecase>>(createUsecasesDto);

        foreach (var usecase in createUsecasesDto)
        {
            if (usecase.TicketSummaryIds != null)
            {
                var ticketSummaries = await _ticketSummaryRepository.GetTicketSummariesByIdsAsync(usecase.TicketSummaryIds).ToListAsync();
                usecases.Where(a => a.TicketSummaries != null).FirstOrDefault()!.TicketSummaries = ticketSummaries;
            }
        }

        await _usecaseRepository.CreateUsecasesAsync(usecases);
        return new CreateUsecasesResponseDto { Usecases = _mapper.Map<List<UsecaseFullDto>>(usecases) };
    }

    ///<inheritdoc cref="IUsecasesService.GetUsecasesAsync(GetUsecasesDto)"/>
    public async Task<PaginationResponseDto<UsecaseFullDto>> GetUsecasesAsync(GetUsecasesDto getUsecases, Guid userIdFromToken)
    {
        ValidateUserPermission(await _usecaseRepository.GetUserIdByTicketIdAsync(getUsecases.TicketId), userIdFromToken);
        var response = await _usecaseRepository.GetUsecasesAsync(getUsecases.PaginationRequest, getUsecases.TicketId);
        return _mapper.Map<PaginationResponseDto<UsecaseFullDto>>(response);
    }

    ///<inheritdoc cref="IUsecasesService.UpdateUsecaseAsync(Guid, UsecaseDataFullDto)"/>
    public async Task<UsecaseFullDto> UpdateUsecaseAsync(Guid usecaseId, UsecaseDataFullDto updateUsecaseDto, Guid userIdFromToken)
    {
        var usecase = await _usecaseRepository.GetUsecaseByIdAsync(usecaseId)
            ?? throw new Exception($"Usecase with id - {usecaseId} was not found");

        ValidateUserPermission(await _usecaseRepository.GetUserIdByTicketIdAsync(usecase.TicketId), userIdFromToken);

        var mappedUsecase = _mapper.Map(updateUsecaseDto, usecase);

        await _usecaseRepository.UpdateUsecaseAsync(mappedUsecase);
        return _mapper.Map<UsecaseFullDto>(usecase);
    }

    ///<inheritdoc cref="IUsecasesService.DeleteUsecasesByTicketIdAsync(Guid, Guid)"/>
    public async Task DeleteUsecasesByTicketIdAsync(Guid ticketId, Guid userIdFromToken)
    {
        ValidateUserPermission(await _usecaseRepository.GetUserIdByTicketIdAsync(ticketId), userIdFromToken);
        await _usecaseRepository.DeleteUsecasesAsync(ticketId);
    }
}
