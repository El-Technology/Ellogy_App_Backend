using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using TicketsManager.BLL.Dtos.TicketShareDtos;
using TicketsManager.BLL.Interfaces;
using TicketsManager.BLL.Interfaces.External;
using TicketsManager.Common.Dtos;
using TicketsManager.Common.Dtos.NotificationDtos;
using TicketsManager.Common.Helpers;
using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.BLL.Services;
public class TicketShareService : ITicketShareService
{
    private readonly ITicketShareRepository _ticketShareRepository;
    private readonly IUserExternalHttpService _userExternalHttpService;
    private readonly IMapper _mapper;
    private readonly IServiceScopeFactory _serviceProvider;

    public TicketShareService(
        ITicketShareRepository ticketShareRepository,
        IMapper mapper,
        IUserExternalHttpService userExternalHttpService,
        IServiceScopeFactory serviceProvider)
    {
        _ticketShareRepository = ticketShareRepository;
        _mapper = mapper;
        _userExternalHttpService = userExternalHttpService;
        _serviceProvider = serviceProvider;
    }

    #region Private methods

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

    private void SendNotificationInBackgroundAsync(Guid ownerId, List<TicketShare> createTicketShareDto)
    {
        _ = Task.Run(async () =>
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var userExternalService = scope.ServiceProvider.GetRequiredService<IUserExternalHttpService>();
            var ticketShareRepository = scope.ServiceProvider.GetRequiredService<ITicketShareRepository>();
            var serviceBusQueue = scope.ServiceProvider.GetRequiredService<IServiceBusQueue>();

            var accessToBuilder = new StringBuilder();

            for (var i = 0; i < createTicketShareDto.Count; i++)
            {
                var share = createTicketShareDto[i];
                var currentStepText = $"{share.TicketCurrentStep.ToString()}";

                if (share.TicketCurrentStep == TicketCurrentStepEnum.General
                    && share.SubStageEnum is not null)
                {
                    currentStepText += $": {share.SubStageEnum.ToString()}";
                }

                accessToBuilder.Append(currentStepText);

                if (i < createTicketShareDto.Count - 1)
                {
                    accessToBuilder.Append("<br>");
                }
            }
            var users = await userExternalService.GetUsersByIdsAsync(new List<Guid>
            {
                ownerId,
                createTicketShareDto.First().SharedUserId
            });

            var ticketTitle = await ticketShareRepository
                .GetTicketTitleByTicketIdAsync(createTicketShareDto.First().TicketId);

            var ownerModel = users.FirstOrDefault(a => a.Id == ownerId) ?? new();

            await serviceBusQueue.SendMessageAsync(
                NotificationHelper.CreateSharingNotification(new SharingNotificationDto
                {
                    ConsumerEmail = users.FirstOrDefault(a => a.Id == createTicketShareDto.First().SharedUserId)!.Email,
                    AccessTo = string.IsNullOrEmpty(accessToBuilder.ToString()) ? "All" : accessToBuilder.ToString(),
                    OwnerEmail = ownerModel.Email,
                    OwnerFistName = ownerModel.FirstName,
                    OwnerLastName = ownerModel.LastName,
                    Permission = createTicketShareDto.First().Permission.ToString(),
                    TicketTitle = ticketTitle ?? string.Empty
                }));
        });
    }

    #endregion

    /// <inheritdoc cref="ITicketShareService.CreateTicketShareAsync" />
    public async Task CreateTicketShareAsync(
        Guid ownerId, CreateTicketShareDto createTicketShareDto)
    {
        if (ownerId == createTicketShareDto.SharedUserId)
            throw new InvalidOperationException("User can't share ticket with himself");

        VerifyPermissionEnum(createTicketShareDto.Permission);
        await VerifyIfUserIsTicketOwnerAsync(ownerId, createTicketShareDto.TicketId);

        var ticketShare = _mapper.Map<CreateTicketShareDto, TicketShare>(createTicketShareDto);
        await _ticketShareRepository.CreateTicketShareAsync(ticketShare);

        SendNotificationInBackgroundAsync(ownerId, new() { ticketShare });
    }

    /// <inheritdoc cref="ITicketShareService.CreateManyTicketSharesAsync" />
    public async Task CreateManyTicketSharesAsync(Guid ownerId, CreateManyTicketShareDto createManyTicketShareDto)
    {
        if (ownerId == createManyTicketShareDto.SharedUserId)
            throw new InvalidOperationException("User can't share ticket with himself");

        VerifyPermissionEnum(createManyTicketShareDto.Permission);
        await VerifyIfUserIsTicketOwnerAsync(ownerId, createManyTicketShareDto.TicketId);

        var ticketShares = _mapper.Map<List<TicketShare>>(createManyTicketShareDto.CreateTicketShareDtos);

        foreach (var ticketShare in ticketShares)
        {
            ticketShare.SharedUserId = createManyTicketShareDto.SharedUserId;
            ticketShare.RevokedAt = createManyTicketShareDto.RevokedAt;
            ticketShare.Permission = createManyTicketShareDto.Permission;
            ticketShare.TicketId = createManyTicketShareDto.TicketId;
            ticketShare.GivenAt = DateTime.UtcNow;
        }

        await _ticketShareRepository.CreateManyTicketSharesAsync(ticketShares);

        SendNotificationInBackgroundAsync(ownerId, ticketShares);
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
