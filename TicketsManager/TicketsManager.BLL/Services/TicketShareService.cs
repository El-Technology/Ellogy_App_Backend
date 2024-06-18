using AutoMapper;
using TicketsManager.BLL.Dtos.TicketShareDtos;
using TicketsManager.BLL.Interfaces;
using TicketsManager.BLL.Interfaces.External;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.BLL.Services;
public class TicketShareService : ITicketShareService
{
    private readonly ITicketShareRepository _ticketShareRepository;
    private readonly IUserExternalHttpService _userExternalHttpService;
    private readonly IMapper _mapper;

    public TicketShareService(
        ITicketShareRepository ticketShareRepository,
        IMapper mapper,
        IUserExternalHttpService userExternalHttpService)
    {
        _ticketShareRepository = ticketShareRepository;
        _mapper = mapper;
        _userExternalHttpService = userExternalHttpService;
    }

    private async Task VerifyIfUserIsTicketOwnerAsync(Guid ownerId, Guid ticketId)
    {
        if (!await _ticketShareRepository.VerifyIfUserIsTicketOwnerAsync(ownerId, ticketId))
            throw new InvalidOperationException("User is not the owner of the ticket");
    }

    private void VerifyPermissionEnum(SharePermissionEnum sharePermissionEnum)
    {
        if (!Enum.IsDefined(typeof(SharePermissionEnum), sharePermissionEnum))
            throw new InvalidOperationException("Invalid permission enum");
    }

    /// <inheritdoc cref="ITicketShareService.CreateTicketShareAsync" />
    public async Task CreateTicketShareAsync(
        Guid ownerId, CreateTicketShareDto createTicketShareDto)
    {
        VerifyPermissionEnum(createTicketShareDto.Permission);
        await VerifyIfUserIsTicketOwnerAsync(ownerId, createTicketShareDto.TicketId);

        if (await _ticketShareRepository.IfPermissionAlreadyGivenToUserAsync(
            createTicketShareDto.TicketId, createTicketShareDto.SharedUserId))
            throw new InvalidOperationException("Permission already given to user");

        var ticketShare = _mapper.Map<CreateTicketShareDto, TicketShare>(createTicketShareDto);
        await _ticketShareRepository.CreateTicketShareAsync(ticketShare);
    }

    /// <inheritdoc cref="ITicketShareService.GetListOfSharesAsync" />
    public async Task<PaginationResponseDto<GetTicketShareDto>> GetListOfSharesAsync(
               Guid ownerId, Guid ticketId, PaginationRequestDto paginationRequestDto)
    {
        await VerifyIfUserIsTicketOwnerAsync(ownerId, ticketId);

        var ticketShares = await _ticketShareRepository.GetTicketSharesAsync(ticketId, paginationRequestDto);
        var mappedTicketShares = _mapper.Map<PaginationResponseDto<GetTicketShareDto>>(ticketShares);

        var users = await _userExternalHttpService.GetUsersByIdsAsync(
            ticketShares.Data.Select(x => x.SharedUserId).ToList());

        var userDtoMap = users.ToDictionary(u => u.Id, u => u);

        foreach (var dto in mappedTicketShares.Data)
            if (userDtoMap.TryGetValue(dto.UserDto.Id, out var userDto))
                dto.UserDto = userDto;

        return mappedTicketShares;
    }

    /// <inheritdoc cref="ITicketShareService.UpdateTicketShareAsync" />
    public async Task UpdateTicketShareAsync(Guid ownerId, Guid ticketShareId, UpdateTicketShareDto updateTicketShareDto)
    {
        VerifyPermissionEnum(updateTicketShareDto.Permission);

        var ticketId = await _ticketShareRepository.GetTicketIdByTicketShareIdAsync(ticketShareId);
        await VerifyIfUserIsTicketOwnerAsync(ownerId, ticketId);

        var mapperTicketShare = _mapper.Map<TicketShare>(updateTicketShareDto);
        await _ticketShareRepository.UpdateTicketShareAsync(ticketShareId, mapperTicketShare);
    }

    /// <inheritdoc cref="ITicketShareService.DeleteTicketShareAsync" />
    public async Task DeleteTicketShareAsync(Guid ownerId, Guid ticketShareId)
    {
        var ticketId = await _ticketShareRepository.GetTicketIdByTicketShareIdAsync(ticketShareId);
        await VerifyIfUserIsTicketOwnerAsync(ownerId, ticketId);

        await _ticketShareRepository.DeleteTicketShareAsync(ticketShareId);
    }
}
