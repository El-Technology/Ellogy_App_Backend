using AutoMapper;
using TicketsManager.BLL.Dtos.TicketVisualizationDtos.FullDtos;
using TicketsManager.BLL.Dtos.TicketVisualizationDtos.UpdateDtos;
using TicketsManager.BLL.Dtos.TicketVisualizationDtos.UsecasesDtos;
using TicketsManager.BLL.Interfaces;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models;

namespace TicketsManager.BLL.Services
{
    public class UsecasesService : IUsecasesService
    {
        private readonly IUsecaseRepository _usecaseRepository;
        private readonly IMapper _mapper;
        public UsecasesService(IUsecaseRepository usecaseRepository, IMapper mapper)
        {
            _usecaseRepository = usecaseRepository;
            _mapper = mapper;
        }

        /// <inheritdoc cref="IUsecasesService.CreateUsecasesAsync(CreateUsecasesDto)"/>
        public async Task<CreateUsecasesResponseDto> CreateUsecasesAsync(CreateUsecasesDto createUsecasesDto)
        {
            var table = _mapper.Map<TicketTable>(createUsecasesDto.TicketTable);
            table.TicketId = createUsecasesDto.TicketId;

            var diagrams = _mapper.Map<List<TicketDiagram>>(createUsecasesDto.TicketDiagrams);
            foreach (var diagram in diagrams)
                diagram.TicketId = createUsecasesDto.TicketId;

            await _usecaseRepository.CreateUsecasesAsync(table, diagrams);

            return new CreateUsecasesResponseDto
            {
                TicketTable = _mapper.Map<TicketTableFullDto>(table),
                TicketDiagrams = _mapper.Map<List<TicketDiagramFullDto>>(diagrams)
            };
        }

        /// <inheritdoc cref="IUsecasesService.GetTableAsync(Guid)"/>
        public async Task<TicketTableFullDto> GetTableAsync(Guid ticketId)
        {
            var table = await _usecaseRepository.GetTableAsync(ticketId);
            return _mapper.Map<TicketTableFullDto>(table);
        }

        /// <inheritdoc cref="IUsecasesService.GetDiagramsAsync(GetDiagramsDto)"/>
        public async Task<PaginationResponseDto<TicketDiagramFullDto>> GetDiagramsAsync(GetDiagramsDto getDiagramsDto)
        {
            var response = await _usecaseRepository.GetDiagramsAsync(getDiagramsDto.PaginationRequest, getDiagramsDto.TicketId);
            var mappedResponse = _mapper.Map<PaginationResponseDto<TicketDiagramFullDto>>(response);
            return mappedResponse;
        }

        /// <inheritdoc cref="IUsecasesService.UpdateTicketTable(Guid, TicketTableUpdateDto)"/>
        public async Task<TicketTableFullDto> UpdateTicketTable(Guid ticketTableId, TicketTableUpdateDto ticketTable)
        {
            var table = await _usecaseRepository.GetTableByIdAsync(ticketTableId)
                ?? throw new Exception($"TicketTable was not found {ticketTableId}");

            var mappedTable = _mapper.Map(ticketTable, table);
            await _usecaseRepository.UpdateTableAsync(mappedTable);
            return _mapper.Map<TicketTableFullDto>(mappedTable);
        }

        /// <inheritdoc cref="IUsecasesService.UpdateTicketDiagram(Guid, TicketDiagramUpdateDto)"/>
        public async Task<TicketDiagramFullDto> UpdateTicketDiagram(Guid ticketDiagramId, TicketDiagramUpdateDto ticketDiagram)
        {
            var table = await _usecaseRepository.GetDiagramByIdAsync(ticketDiagramId)
                 ?? throw new Exception($"TicketDiagram was not found {ticketDiagramId}");

            var mappedTable = _mapper.Map(ticketDiagram, table);
            await _usecaseRepository.UpdateDiagramAsync(mappedTable);
            return _mapper.Map<TicketDiagramFullDto>(mappedTable);
        }
    }
}
