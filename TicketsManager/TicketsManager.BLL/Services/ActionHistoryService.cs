using AutoMapper;
using TicketsManager.BLL.Dtos.ActionHistoryDtos;
using TicketsManager.BLL.Interfaces;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models;

namespace TicketsManager.BLL.Services
{
    public class ActionHistoryService : IActionHistoryService
    {
        private readonly IActionHistoryRepository _actionHistoryRepository;
        private readonly IMapper _mapper;
        private readonly ITicketsRepository _ticketsRepository;

        public ActionHistoryService(IActionHistoryRepository actionHistoryRepository, IMapper mapper, ITicketsRepository ticketsRepository)
        {
            _ticketsRepository = ticketsRepository;
            _mapper = mapper;
            _actionHistoryRepository = actionHistoryRepository;
        }

        public async Task<PaginationResponseDto<ActionHistory>> GetActionHistoriesAsync(Guid ticketId, SearchHistoryRequestDto searchHistoryRequestDto)
        {
            CheckTicketCurrentStepEnum(searchHistoryRequestDto.TicketCurrentStepEnum);
            await CheckIfTicketExist(ticketId);

            var response = await _actionHistoryRepository.GetActionHistoriesAsync(ticketId, searchHistoryRequestDto.TicketCurrentStepEnum, searchHistoryRequestDto.Pagination);
            return response;
        }

        public async Task CreateActionHistoryAsync(CreateActionHistoryDto createActionHistoryDto)
        {
            CheckActionHistoryEnum(createActionHistoryDto.ActionHistoryEnum);
            CheckTicketCurrentStepEnum(createActionHistoryDto.TicketCurrentStepEnum);

            await CheckIfTicketExist(createActionHistoryDto.TicketId);

            await _actionHistoryRepository.CreateActionHistoryAsync(_mapper.Map<ActionHistory>(createActionHistoryDto));
        }

        private async Task CheckIfTicketExist(Guid ticketId)
        {
            _ = await _ticketsRepository.GetTicketByIdAsync(ticketId)
                ?? throw new Exception($"Ticket with id - {ticketId} was not found");
        }

        private void CheckTicketCurrentStepEnum(TicketCurrentStepEnum ticketCurrentStepEnum)
        {
            if (!Enum.IsDefined(typeof(TicketCurrentStepEnum), ticketCurrentStepEnum))
                throw new Exception("Wrong emun");
        }

        private void CheckActionHistoryEnum(ActionHistoryEnum actionHistoryEnum)
        {
            if (!Enum.IsDefined(typeof(ActionHistoryEnum), actionHistoryEnum))
                throw new Exception("Wrong emun");
        }
    }
}
