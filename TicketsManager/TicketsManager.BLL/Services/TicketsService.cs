using AutoMapper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlToOpenXml;
using System.Text;
using TicketsManager.BLL.Dtos.MessageDtos;
using TicketsManager.BLL.Dtos.TicketDtos;
using TicketsManager.BLL.Exceptions;
using TicketsManager.BLL.Interfaces;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Exceptions;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models;

namespace TicketsManager.BLL.Services;

public class TicketsService : ITicketsService
{
    private readonly IMapper _mapper;
    private readonly ITicketsRepository _ticketsRepository;

    public TicketsService(IMapper mapper, ITicketsRepository ticketsRepository)
    {
        _mapper = mapper;
        _ticketsRepository = ticketsRepository;
    }

    private static void ValidateUserPermission(Guid inputUserId, Guid userIdFromToken)
    {
        if (inputUserId != userIdFromToken)
            throw new Exception("You don't have permission to access another user data");
    }

    private static string ConvertBase64ToString(string base64)
    {
        var htmlBytes = Convert.FromBase64String(base64);
        var decodedHtml = Encoding.UTF8.GetString(htmlBytes);

        return decodedHtml;
    }

    private Ticket MapCreateTicket(TicketCreateRequestDto createTicketRequest, Guid userId)
    {
        var mappedTicket = _mapper.Map<Ticket>(createTicketRequest);
        mappedTicket.UserId = userId;

        foreach (var message in mappedTicket.TicketMessages)
            message.TicketId = mappedTicket.Id;

        foreach (var summary in mappedTicket.TicketSummaries)
        {
            summary.TicketId = mappedTicket.Id;
            summary.Ticket = mappedTicket;
        }

        return mappedTicket;
    }

    private static void CheckTicketEnums(TicketStatusEnum ticketStatusEnum,
        TicketCurrentStepEnum ticketCurrentStepEnum)
    {
        if (!Enum.IsDefined(typeof(TicketStatusEnum), ticketStatusEnum)
            && !Enum.IsDefined(typeof(TicketCurrentStepEnum), ticketCurrentStepEnum))
            throw new Exception("Wrong enum");
    }

    private static void CheckMessageCreateEnums(List<MessageDto> messages)
    {
        foreach (var message in messages)
        {
            if (message.Action is null) return;

            CheckEnumValue(message.Action.State, typeof(MessageActionStateEnum), "Action State");
            CheckEnumValue(message.Action.Type, typeof(MessageActionTypeEnum), "Action Type");
            CheckEnumValue(message.Stage, typeof(MessageStageEnum), "Message Stage");
        }
    }

    private static void CheckMessageUpdateEnums(List<MessageResponseDto> messages)
    {
        foreach (var message in messages)
        {
            if (message.Action is null) return;

            CheckEnumValue(message.Action.State, typeof(MessageActionStateEnum), "Action State");
            CheckEnumValue(message.Action.Type, typeof(MessageActionTypeEnum), "Action Type");
            CheckEnumValue(message.Stage, typeof(MessageStageEnum), "Message Stage");
        }
    }

    private static void CheckEnumValue<TEnum>(TEnum? enumValue, Type enumType, string propertyName)
        where TEnum : struct, Enum
    {
        if (enumValue.HasValue && !Enum.IsDefined(enumType, enumValue.Value))
        {
            throw new Exception($"Invalid {propertyName} enum value");
        }
    }

    /// <inheritdoc cref="ITicketsService.GetTicketsAsync(Guid, PaginationRequestDto, Guid)"/>
    public async Task<PaginationResponseDto<TicketResponseDto>> GetTicketsAsync(
        Guid userId, PaginationRequestDto paginateRequest, Guid userIdFromToken)
    {
        ValidateUserPermission(userId, userIdFromToken);

        try
        {
            var tickets = await _ticketsRepository.GetTicketsAsync(userId, paginateRequest);
            return _mapper.Map<PaginationResponseDto<TicketResponseDto>>(tickets);
        }
        catch (EntityNotFoundException ex)
        {
            throw new UserNotFoundException(userId, ex);
        }
    }

    /// <inheritdoc cref="ITicketsService.SearchTicketsByNameAsync(Guid, SearchTicketsRequestDto, Guid)"/>
    public async Task<PaginationResponseDto<TicketResponseDto>> SearchTicketsByNameAsync(
        Guid userId, SearchTicketsRequestDto searchRequest, Guid userIdFromToken)
    {
        ValidateUserPermission(userId, userIdFromToken);

        try
        {
            var findTickets = await _ticketsRepository.FindTicketsAsync(userId, searchRequest);
            return _mapper.Map<PaginationResponseDto<TicketResponseDto>>(findTickets);
        }
        catch (EntityNotFoundException ex)
        {
            throw new UserNotFoundException(userId, ex);
        }
    }

    /// <inheritdoc cref="ITicketsService.CreateTicketAsync(TicketCreateRequestDto, Guid, Guid)"/>
    public async Task<TicketResponseDto> CreateTicketAsync(
        TicketCreateRequestDto createTicketRequest, Guid userId, Guid userIdFromToken)
    {
        CheckTicketEnums(createTicketRequest.Status, createTicketRequest.CurrentStep);
        CheckMessageCreateEnums(createTicketRequest.Messages);
        ValidateUserPermission(userId, userIdFromToken);

        var mappedTicket = MapCreateTicket(createTicketRequest, userId);
        await _ticketsRepository.CreateTicketAsync(mappedTicket);

        return _mapper.Map<TicketResponseDto>(mappedTicket);
    }

    /// <inheritdoc cref="ITicketsService.DeleteTicketAsync(Guid, Guid)"/>
    public async Task DeleteTicketAsync(Guid ticketId, Guid userIdFromToken)
    {
        var ticket = await _ticketsRepository.GetTicketByIdAsync(ticketId)
            ?? throw new TicketNotFoundException(ticketId);

        ValidateUserPermission(ticket.UserId, userIdFromToken);

        await _ticketsRepository.DeleteTicketAsync(ticket.Id);
    }

    /// <inheritdoc cref="ITicketsService.UpdateTicketAsync(Guid, TicketUpdateRequestDto, Guid)"/>
    public async Task<TicketResponseDto> UpdateTicketAsync(
        Guid ticketId, TicketUpdateRequestDto ticketUpdate, Guid userIdFromToken)
    {
        CheckTicketEnums(ticketUpdate.Status, ticketUpdate.CurrentStep);
        CheckMessageUpdateEnums(ticketUpdate.Messages);

        var ticket = await _ticketsRepository.GetTicketByIdAsync(ticketId)
                     ?? throw new TicketNotFoundException(ticketId);

        ValidateUserPermission(ticket.UserId, userIdFromToken);

        var mappedTicket = _mapper.Map(ticketUpdate, ticket);
        await _ticketsRepository.CheckTicketUpdateIds(mappedTicket);
        await _ticketsRepository.UpdateTicketAsync(mappedTicket);

        return _mapper.Map<TicketResponseDto>(mappedTicket);
    }

    ///<inheritdoc cref="ITicketsService.DownloadAsDoc(string[])"/>
    public byte[] DownloadAsDoc(string[] base64Data)
    {
        using var memoryStream = new MemoryStream();
        using (var package = WordprocessingDocument.Create(memoryStream,
                            WordprocessingDocumentType.Document))
        {
            var mainPart = package.MainDocumentPart;
            if (mainPart == null)
            {
                mainPart = package.AddMainDocumentPart();
                new Document(new Body()).Save(mainPart);
            }
            var converter = new HtmlConverter(mainPart);

            foreach (var page in base64Data)
            {
                var htmlContent = ConvertBase64ToString(page);
                converter.ParseHtml(htmlContent);
            }

            mainPart.Document.Save();
        }
        return memoryStream.ToArray();
    }
}
