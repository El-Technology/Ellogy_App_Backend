using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TicketsManager.BLL.Dtos.TicketSummaryDtos;
using TicketsManager.BLL.Interfaces;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models;

namespace TicketsManager.BLL.Services;

public class TicketSummaryService : ITicketSummaryService
{
    private readonly IMapper _mapper;
    private readonly ITicketSummaryRepository _ticketSummaryRepository;

    public TicketSummaryService(ITicketSummaryRepository ticketSummaryRepository, IMapper mapper)
    {
        _mapper = mapper;
        _ticketSummaryRepository = ticketSummaryRepository;
    }

    /// <inheritdoc cref="ITicketSummaryService.GetTicketSummariesByTicketIdAsync" />
    public async Task<List<TicketSummaryFullDto>> GetTicketSummariesByTicketIdAsync(Guid ticketId)
    {
        return _mapper.Map<List<TicketSummaryFullDto>>(await _ticketSummaryRepository
            .GetTicketSummariesByTicketIdAsync(ticketId).ToListAsync());
    }

    /// <inheritdoc cref="ITicketSummaryService.CreateTicketSummariesAsync" />
    public async Task<List<TicketSummaryFullDto>> CreateTicketSummariesAsync(
        List<TicketSummaryCreateDto> ticketSummaries)
    {
        var mappedTicketSummaries = _mapper.Map<List<TicketSummary>>(ticketSummaries);
        await _ticketSummaryRepository.CreateTicketSummariesAsync(mappedTicketSummaries);

        return _mapper.Map<List<TicketSummaryFullDto>>(mappedTicketSummaries);
    }

    /// <inheritdoc cref="ITicketSummaryService.UpdateTicketSummariesAsync" />
    public async Task<List<TicketSummaryFullDto>> UpdateTicketSummariesAsync(
        List<TicketSummaryFullDto> ticketSummaries)
    {
        var mappedTicketSummaries = _mapper.Map<List<TicketSummary>>(ticketSummaries);
        await _ticketSummaryRepository.UpdateTicketSummariesAsync(mappedTicketSummaries);

        return _mapper.Map<List<TicketSummaryFullDto>>(mappedTicketSummaries);
    }

    /// <inheritdoc cref="ITicketSummaryService.DeleteTicketSummariesAsync" />
    public async Task DeleteTicketSummariesAsync(Guid ticketId)
    {
        await _ticketSummaryRepository.DeleteTicketSummariesAsync(ticketId);
    }
}