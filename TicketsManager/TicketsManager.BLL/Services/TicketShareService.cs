using AutoMapper;
using TicketsManager.BLL.Dtos.TicketShareDtos;
using TicketsManager.BLL.Interfaces;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.BLL.Services;
public class TicketShareService : ITicketShareService
{
    private readonly ITicketShareRepository _ticketShareRepository;
    private readonly IMapper _mapper;

    public TicketShareService(
        ITicketShareRepository ticketShareRepository,
        IMapper mapper)
    {
        _ticketShareRepository = ticketShareRepository;
        _mapper = mapper;
    }

    private async Task VerifyIfUserIsTicketOwnerAsync(Guid ownerId, Guid ticketId)
    {
        if (!await _ticketShareRepository.VerifyIfUserIsTicketOwnerAsync(ownerId, ticketId))
            throw new InvalidOperationException("User is not the owner of the ticket");
    }

    /// <inheritdoc cref="ITicketShareService.CreateTicketShareAsync" />
    public async Task<TicketShare> CreateTicketShareAsync(
        Guid ownerId, CreateTicketShareDto createTicketShareDto)
    {
        await VerifyIfUserIsTicketOwnerAsync(ownerId, createTicketShareDto.TicketId);

        var ticketShare = _mapper.Map<CreateTicketShareDto, TicketShare>(createTicketShareDto);
        await _ticketShareRepository.CreateTicketShareAsync(ticketShare);

        return ticketShare;
    }

    /// <inheritdoc cref="ITicketShareService.GetListOfSharesAsync" />
    public async Task<PaginationResponseDto<TicketShare>> GetListOfSharesAsync(
               Guid ticketId, PaginationRequestDto paginationRequestDto)
    {
        //todo verify permission or ownership
        return await _ticketShareRepository.GetTicketSharesAsync(ticketId, paginationRequestDto);
    }

    /// <inheritdoc cref="ITicketShareService.UpdateTicketShareAsync" />
    public async Task UpdateTicketShareAsync(TicketShare ticketShare)
    {
        //todo verify permission or ownership
        await _ticketShareRepository.UpdateTicketShareAsync(ticketShare);
    }

    /// <inheritdoc cref="ITicketShareService.DeleteTicketShareAsync" />
    public async Task DeleteTicketShareAsync(Guid ownerId, Guid ticketShareId)
    {
        //todo verify permission or ownership
        await _ticketShareRepository.DeleteTicketShareAsync(ticketShareId);
    }
}
