using AutoMapper;
using TicketsManager.BLL.Dtos.MessageDtos;
using TicketsManager.BLL.Exceptions;
using TicketsManager.BLL.Interfaces;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Dtos;
using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.BLL.Services;
public class TicketMessageService : ITicketMessageService
{
    private readonly ITicketMessageRepository _ticketMessageRepository;
    private readonly IMapper _mapper;
    private readonly ITicketShareRepository _ticketShareRepository;
    public TicketMessageService(
        ITicketMessageRepository ticketMessageRepository,
        IMapper mapper,
        ITicketShareRepository ticketShareRepository)
    {
        _ticketMessageRepository = ticketMessageRepository;
        _mapper = mapper;
        _ticketShareRepository = ticketShareRepository;
    }

    /// <inheritdoc cref="ITicketMessageService.GetTicketMessagesByTicketIdAsync" />
    public async Task<PaginationResponseDto<MessageResponseDto>> GetTicketMessagesByTicketIdAsync(
        GetMessageDto getMessage)
    {
        await _ticketShareRepository.CheckIfUserHaveAccessToComponentStrictAsync(
            getMessage.TicketId,
            getMessage.UserId,
            getMessage.MessageStage,
            SharePermissionEnum.Read);

        var messages = await _ticketMessageRepository
            .GetTicketMessagesByTicketIdAsync(getMessage);

        if (messages.RecordsReturned == default)
            throw new MessageNotFoundException();

        return _mapper.Map<PaginationResponseDto<MessageResponseDto>>(messages);
    }

    /// <inheritdoc cref="ITicketMessageService.UpdateMessageAsync" />
    public async Task<MessageResponseDto> UpdateMessageAsync(
        MessageResponseDto messageRequestDto, Guid ticketId, Guid userId)
    {
        await _ticketShareRepository.CheckIfUserHaveAccessToComponentStrictAsync(
            ticketId,
            userId,
            messageRequestDto.Stage,
            SharePermissionEnum.ReadWrite);


        await _ticketShareRepository.CheckIfUserHaveAccessToSubStageStrictAsync(
            ticketId,
            userId,
            messageRequestDto.SubStage,
            SharePermissionEnum.ReadWrite);

        var message = _mapper.Map<Message>(messageRequestDto);
        message.TicketId = ticketId;

        await _ticketMessageRepository.UpdateMessageAsync(message);

        return _mapper.Map<MessageResponseDto>(message);
    }

    /// <inheritdoc cref="ITicketMessageService.UpdateRangeMessagesAsync" />
    public async Task<IEnumerable<MessageResponseDto>> UpdateRangeMessagesAsync(
        List<MessageResponseDto> messageResponseDtos, Guid ticketId, Guid userId)
    {
        var stagesAndSubStages = messageResponseDtos
            .Select(msg => new { msg.Stage, msg.SubStage })
            .Distinct()
            .ToList();

        foreach (var item in stagesAndSubStages)
        {
            await _ticketShareRepository.CheckIfUserHaveAccessToComponentStrictAsync(
                ticketId,
                userId,
                item.Stage,
                SharePermissionEnum.ReadWrite);

            if (item.SubStage != null)
            {
                await _ticketShareRepository.CheckIfUserHaveAccessToSubStageStrictAsync(
                    ticketId,
                    userId,
                    item.SubStage,
                    SharePermissionEnum.ReadWrite);
            }
        }

        var messages = _mapper.Map<List<Message>>(messageResponseDtos);

        foreach (var message in messages)
            message.TicketId = ticketId;

        await _ticketMessageRepository.UpdateRangeMessagesAsync(messages);

        return _mapper.Map<IEnumerable<MessageResponseDto>>(messages);
    }

    /// <inheritdoc cref="ITicketMessageService.CreateMessageAsync" />
    public async Task<MessageResponseDto> CreateMessageAsync(
        MessageDto messageDto, Guid ticketId, Guid userId)
    {
        await _ticketShareRepository.CheckIfUserHaveAccessToSubStageStrictAsync(
            ticketId,
            userId,
            messageDto.SubStage,
            SharePermissionEnum.ReadWrite);

        var message = _mapper.Map<Message>(messageDto);
        message.TicketId = ticketId;

        await _ticketMessageRepository.CreateMessageAsync(message);

        return _mapper.Map<MessageResponseDto>(message);
    }

    /// <inheritdoc cref="ITicketMessageService.CreateRangeMessagesAsync" />
    public async Task<IEnumerable<MessageResponseDto>> CreateRangeMessagesAsync(
        List<MessageDto> messageDtos, Guid ticketId, Guid userId)
    {
        var uniqueSubStages = messageDtos
            .Select(msg => msg.SubStage)
            .Distinct()
            .ToList();

        foreach (var subStage in uniqueSubStages)
            await _ticketShareRepository.CheckIfUserHaveAccessToSubStageStrictAsync(
                ticketId,
                userId,
                subStage,
                SharePermissionEnum.ReadWrite);

        var messages = _mapper.Map<List<Message>>(messageDtos);

        foreach (var message in messages)
            message.TicketId = ticketId;

        await _ticketMessageRepository.CreateRangeMessagesAsync(messages);

        return _mapper.Map<IEnumerable<MessageResponseDto>>(messages);
    }

    /// <inheritdoc cref="ITicketMessageService.DeleteMessageAsync" />
    public async Task DeleteMessageAsync(Guid messageId, Guid userId, Guid ticketId)
    {
        var message = await _ticketMessageRepository.GetMessageAsync(messageId, userId)
            ?? throw new MessageNotFoundException();

        await _ticketShareRepository.CheckIfUserHaveAccessToSubStageStrictAsync(
            ticketId,
            userId,
            message.SubStage,
            SharePermissionEnum.Manage);

        await _ticketMessageRepository.DeleteMessageAsync(message);
    }

    /// <inheritdoc cref="ITicketMessageService.DeleteRangeMessagesAsync" />
    public async Task DeleteRangeMessagesAsync(
        List<Guid> messageIds, Guid userId, Guid ticketId)
    {
        var messages = await _ticketMessageRepository.GetRangeMessagesAsync(
            messageIds, userId);

        if (messages is null
            || messages.Count != messageIds.Count
            || messages.Count == default)
            throw new MessageNotFoundException();

        var uniqueSubStages = messages
            .Select(msg => msg.SubStage)
            .Distinct()
            .ToList();

        foreach (var subStage in uniqueSubStages)
            await _ticketShareRepository.CheckIfUserHaveAccessToSubStageStrictAsync(
                ticketId,
                userId,
                subStage,
                SharePermissionEnum.ReadWrite);

        await _ticketMessageRepository.DeleteRangeMessagesAsync(messages);
    }
}
