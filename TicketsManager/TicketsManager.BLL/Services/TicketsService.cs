﻿using AutoMapper;
using TicketsManager.BLL.Dtos.TicketDtos;
using TicketsManager.BLL.Exceptions;
using TicketsManager.BLL.Interfaces;
using TicketsManager.Common.Helpers.Pagination;
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

    public async Task<TicketResponseDto> CreateTicketAsync(TicketCreateRequestDto createTicketRequest, Guid userId)
    {
        var user = await _userRepository.GetUserAsync(userId) ?? throw new UserNotFoundException(userId);
        var mappedTicket = MapCreateTicket(createTicketRequest, user);

        await _ticketsRepository.CreateTicketAsync(mappedTicket);

        var returnTicket = _mapper.Map<TicketResponseDto>(mappedTicket);
        return returnTicket;
    }

    private Ticket MapCreateTicket(TicketCreateRequestDto createTicketRequest, User user)
    {
        var mappedTicket = _mapper.Map<Ticket>(createTicketRequest);
        mappedTicket.User = user;

        foreach (var message in mappedTicket.TicketMessages)
            message.Ticket = mappedTicket;

        return mappedTicket;
    }

    public async Task DeleteTicketAsync(Guid id)
    {
        var ticket = await _ticketsRepository.GetTicketByIdAsync(id)
                     ?? throw new TicketNotFoundException(id);

        await _ticketsRepository.DeleteTicketAsync(ticket);
    }

    public async Task<TicketResponseDto> UpdateTicketAsync(TicketUpdateRequestDto ticketUpdate)
    {
        var ticket = await _ticketsRepository.GetTicketByIdAsync(ticketUpdate.Id)
                     ?? throw new TicketNotFoundException(ticketUpdate.Id);

        var mappedTicket = _mapper.Map(ticketUpdate, ticket);
        await _ticketsRepository.UpdateTicketAsync(mappedTicket);

        return _mapper.Map<TicketResponseDto>(mappedTicket);
    }
}
