using AutoMapper;
using TicketsManager.BLL.Dtos.TicketDtos;
using TicketsManager.BLL.Exceptions;
using TicketsManager.BLL.Interfaces;
using TicketsManager.DAL.Exceptions;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models;

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

    public async Task<ICollection<TicketResponseDto>> GetAllTicketsAsync(Guid userId)
    {
        try
        {
            var tickets = await _ticketsRepository.GetAllTicketsAsync(userId);
            return _mapper.Map<ICollection<TicketResponseDto>>(tickets);
        }
        catch (EntityNotFoundException ex)
        {
            throw new UserNotFoundException(userId, ex);
        }
    }

    public async Task<TicketResponseDto> CreateTicketAsync(TicketCreateRequestDto createTicketRequest, Guid userId)
    {
        var user = await _userRepository.GetUserAsync(userId) ?? throw new UserNotFoundException(userId);
        var mappedTicket = MapTicket(createTicketRequest, user);

        await _ticketsRepository.CreateTicketAsync(mappedTicket);

        var returnTicket = _mapper.Map<TicketResponseDto>(mappedTicket);
        return returnTicket;
    }

    private Ticket MapTicket(TicketCreateRequestDto createTicketRequest, User user)
    {
        var mappedTicket = _mapper.Map<Ticket>(createTicketRequest);
        mappedTicket.User = user;

        foreach (var message in mappedTicket.TicketMessages)
            message.Ticket = mappedTicket;

        return mappedTicket;
    }
}
