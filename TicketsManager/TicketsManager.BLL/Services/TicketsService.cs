using AutoMapper;
using TicketsManager.BLL.Dtos.TicketDtos;
using TicketsManager.BLL.Exceptions;
using TicketsManager.BLL.Interfaces;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Exceptions;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models;
using HtmlToOpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using PdfSharpCore.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using System.Text;
using DocumentFormat.OpenXml.Wordprocessing;

namespace TicketsManager.BLL.Services;

public class TicketsService : ITicketsService
{
    private readonly IMapper _mapper;
    private readonly ITicketsRepository _ticketsRepository;
    private readonly IUserRepository _userRepository;

    public TicketsService(IMapper mapper, ITicketsRepository ticketsRepository, IUserRepository userRepository)
    {
        _mapper = mapper;
        _ticketsRepository = ticketsRepository;
        _userRepository = userRepository;
    }
    private Ticket MapCreateTicket(TicketCreateRequestDto createTicketRequest, User user)
    {
        var mappedTicket = _mapper.Map<Ticket>(createTicketRequest);
        mappedTicket.UserId = user.Id;

        foreach (var message in mappedTicket.TicketMessages)
            message.TicketId = mappedTicket.Id;

        foreach (var summary in mappedTicket.TicketSummaries)
        {
            summary.TicketId = mappedTicket.Id;
            summary.Ticket = mappedTicket;
        }

        return mappedTicket;
    }

    /// <inheritdoc cref="ITicketsService.GetTicketsAsync(Guid, PaginationRequestDto)"/>
    public async Task<PaginationResponseDto<TicketResponseDto>> GetTicketsAsync(Guid userId, PaginationRequestDto paginateRequest)
    {
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

    /// <inheritdoc cref="ITicketsService.SearchTicketsByNameAsync(Guid, SearchTicketsRequestDto)"/>
    public async Task<PaginationResponseDto<TicketResponseDto>> SearchTicketsByNameAsync(Guid userId, SearchTicketsRequestDto searchRequest)
    {
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

    /// <inheritdoc cref="ITicketsService.CreateTicketAsync(TicketCreateRequestDto, Guid)"/>
    public async Task<TicketResponseDto> CreateTicketAsync(TicketCreateRequestDto createTicketRequest, Guid userId)
    {
        var user = await _userRepository.GetUserAsync(userId) ?? throw new UserNotFoundException(userId);
        var mappedTicket = MapCreateTicket(createTicketRequest, user);

        await _ticketsRepository.CreateTicketAsync(mappedTicket);

        var returnTicket = _mapper.Map<TicketResponseDto>(mappedTicket);
        return returnTicket;
    }

    /// <inheritdoc cref="ITicketsService.DeleteTicketAsync(Guid)"/>
    public async Task DeleteTicketAsync(Guid id)
    {
        var ticket = await _ticketsRepository.GetTicketByIdAsync(id)
                     ?? throw new TicketNotFoundException(id);

        await _ticketsRepository.DeleteTicketAsync(ticket);
    }

    /// <inheritdoc cref="ITicketsService.UpdateTicketAsync(Guid, TicketUpdateRequestDto)"/>
    public async Task<TicketResponseDto> UpdateTicketAsync(Guid id, TicketUpdateRequestDto ticketUpdate)
    {
        var ticket = await _ticketsRepository.GetTicketByIdAsync(id)
                     ?? throw new TicketNotFoundException(id);

        var mappedTicket = _mapper.Map(ticketUpdate, ticket);
        await _ticketsRepository.UpdateTicketAsync(mappedTicket);

        return _mapper.Map<TicketResponseDto>(mappedTicket);
    }

    private string ConvertBase64ToString(string base64)
    {
        var htmlBytes = Convert.FromBase64String(base64);
        var decodedHtml = Encoding.UTF8.GetString(htmlBytes);
        return decodedHtml;
    }

    ///<inheritdoc cref="ITicketsService.DownloadAsPdfAsync(string[])"/>
    public async Task<byte[]> DownloadAsPdfAsync(string[] base64Data)
    {
        var document = new PdfDocument();
        foreach (var page in base64Data)
        {
            var htmlContent = ConvertBase64ToString(page);
            PdfGenerator.AddPdfPages(document, htmlContent, PdfSharpCore.PageSize.A4);
        }
        using var memoryStream = new MemoryStream();
        document.Save(memoryStream);
        var response = memoryStream.ToArray();
        return response;
    }

    ///<inheritdoc cref="ITicketsService.DownloadAsDocAsync(string[])"/>
    public async Task<byte[]> DownloadAsDocAsync(string[] base64Data)
    {
        using var memoryStream = new MemoryStream();
        using (var package = WordprocessingDocument.Create(memoryStream,
                             WordprocessingDocumentType.Document))
        {
            MainDocumentPart mainPart = package.MainDocumentPart;
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
