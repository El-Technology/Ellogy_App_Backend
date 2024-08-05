using AutoMapper;
using TicketsManager.BLL.Dtos.TicketUsecaseDtos.FullDtos;
using TicketsManager.BLL.Dtos.TicketUsecaseDtos.UsecasesDtos;
using TicketsManager.BLL.Interfaces;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.UsecaseModels;

namespace TicketsManager.BLL.Services;

public class UsecasesService : IUsecasesService
{
    private readonly IUsecaseRepository _usecaseRepository;
    private readonly IMapper _mapper;
    private readonly ITicketShareRepository _ticketShareRepository;
    public UsecasesService(
        IUsecaseRepository usecaseRepository,
        IMapper mapper,
        ITicketShareRepository ticketShareRepository)
    {
        _usecaseRepository = usecaseRepository;
        _mapper = mapper;
        _ticketShareRepository = ticketShareRepository;
    }

    #region Private methods

    private async Task ValidateUserPermissionAsync(
        Guid ticketId, Guid userIdFromToken, SharePermissionEnum sharePermissionEnum)
    {
        await _ticketShareRepository.CheckIfUserHaveAccessToComponentStrictAsync(
            ticketId,
            userIdFromToken,
            TicketCurrentStepEnum.Usecases,
            sharePermissionEnum);
    }

    #endregion

    ///<inheritdoc cref="IUsecasesService.CreateUsecasesAsync(List{CreateUsecasesDto},Guid)"/>
    public async Task<CreateUsecasesResponseDto> CreateUsecasesAsync(List<CreateUsecasesDto> createUsecasesDto, Guid userIdFromToken)
    {
        await ValidateUserPermissionAsync(
            createUsecasesDto.FirstOrDefault()!.TicketId,
            userIdFromToken,
            SharePermissionEnum.ReadWrite);

        var usecases = _mapper.Map<List<Usecase>>(createUsecasesDto);

        foreach (var usecaseGroup in usecases.GroupBy(u => u.TicketId))
        {
            var lastOrder = await _usecaseRepository.GetLastOrderForUsecaseByTicketIdAsync(usecaseGroup.Key);

            foreach (var usecase in usecaseGroup)
                usecase.Order = ++lastOrder;
        }

        await _usecaseRepository.CreateUsecasesAsync(usecases);
        return new CreateUsecasesResponseDto { Usecases = _mapper.Map<List<UsecaseFullDto>>(usecases) };
    }

    ///<inheritdoc cref="IUsecasesService.GetUsecasesAsync(GetUsecasesDto, Guid)"/>
    public async Task<PaginationResponseDto<UsecaseFullDto>> GetUsecasesAsync(
        GetUsecasesDto getUsecases, Guid userIdFromToken)
    {
        await ValidateUserPermissionAsync(
            getUsecases.TicketId,
            userIdFromToken,
            SharePermissionEnum.Read);

        var response = await _usecaseRepository
            .GetUsecasesAsync(getUsecases.PaginationRequest, getUsecases.TicketId);

        return _mapper.Map<PaginationResponseDto<UsecaseFullDto>>(response);
    }

    ///<inheritdoc cref="IUsecasesService.UpdateUsecaseAsync(Guid, UsecaseDataFullDto, Guid)"/>
    public async Task<UsecaseFullDto> UpdateUsecaseAsync(
        Guid usecaseId, UsecaseDataFullDto updateUsecaseDto, Guid userIdFromToken)
    {
        var usecase = await _usecaseRepository.GetUsecaseByIdAsync(usecaseId)
            ?? throw new Exception($"Usecase with id - {usecaseId} was not found");

        await ValidateUserPermissionAsync(
            usecase.TicketId,
            userIdFromToken,
            SharePermissionEnum.ReadWrite);

        var mappedUsecase = _mapper.Map(updateUsecaseDto, usecase);
        await _usecaseRepository.UpdateUsecaseAsync(mappedUsecase);

        return _mapper.Map<UsecaseFullDto>(usecase);
    }

    ///<inheritdoc cref="IUsecasesService.DeleteUsecasesByTicketIdAsync(Guid, Guid)"/>
    public async Task DeleteUsecasesByTicketIdAsync(Guid ticketId, Guid userIdFromToken)
    {
        await ValidateUserPermissionAsync(
            ticketId,
            userIdFromToken,
            SharePermissionEnum.Manage);

        await _usecaseRepository.DeleteUsecasesAsync(ticketId);
    }
}
