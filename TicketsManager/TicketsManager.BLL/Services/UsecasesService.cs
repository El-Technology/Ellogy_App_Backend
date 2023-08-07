using AutoMapper;
using TicketsManager.BLL.Dtos.TicketUsecaseDtos.FullDtos;
using TicketsManager.BLL.Dtos.TicketUsecaseDtos.UsecasesDtos;
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

        ///<inheritdoc cref="IUsecasesService.CreateUsecasesAsync(List{CreateUsecasesDto})"/>
        public async Task<CreateUsecasesResponseDto> CreateUsecasesAsync(List<CreateUsecasesDto> createUsecasesDto)
        {
            var usecases = _mapper.Map<List<Usecase>>(createUsecasesDto);
            await _usecaseRepository.CreateUsecasesAsync(usecases);
            return new CreateUsecasesResponseDto { Usecases = _mapper.Map<List<UsecaseFullDto>>(usecases) };
        }

        ///<inheritdoc cref="IUsecasesService.GetUsecasesAsync(GetUsecasesDto)"/>
        public async Task<PaginationResponseDto<UsecaseFullDto>> GetUsecasesAsync(GetUsecasesDto getUsecases)
        {
            var response = await _usecaseRepository.GetUsecasesAsync(getUsecases.PaginationRequest, getUsecases.TicketId);
            return _mapper.Map<PaginationResponseDto<UsecaseFullDto>>(response);
        }

        ///<inheritdoc cref="IUsecasesService.UpdateUsecaseAsync(Guid, UsecaseDataFullDto)"/>
        public async Task<UsecaseFullDto> UpdateUsecaseAsync(Guid usecaseId, UsecaseDataFullDto updateUsecaseDto)
        {
            var usecase = await _usecaseRepository.GetUsecaseByIdAsync(usecaseId)
                ?? throw new Exception($"Usecase with id - {usecaseId} was not found");

            var mappedUsecase = _mapper.Map(updateUsecaseDto, usecase);

            await _usecaseRepository.UpdateUsecaseAsync(mappedUsecase);
            return _mapper.Map<UsecaseFullDto>(usecase);
        }
    }
}
