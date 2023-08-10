using TicketsManager.BLL.Dtos.TicketUsecaseDtos.FullDtos;
using TicketsManager.BLL.Dtos.TicketUsecaseDtos.UsecasesDtos;
using TicketsManager.Common.Dtos;

namespace TicketsManager.BLL.Interfaces
{
    /// <summary>
    /// Interface for managing use cases related operations.
    /// </summary>
    public interface IUsecasesService
    {
        /// <summary>
        /// Creates multiple use cases asynchronously.
        /// </summary>
        /// <param name="createUsecasesDto">The list of data to create use cases.</param>
        /// <returns>A task representing the asynchronous operation. The response containing information about the created use cases.</returns>
        Task<CreateUsecasesResponseDto> CreateUsecasesAsync(List<CreateUsecasesDto> createUsecasesDto);

        /// <summary>
        /// Deletes all usecases by ticket id
        /// </summary>
        /// <param name="ticketId">ID of the ticket, under which all use cases are located.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteUsecasesByTicketIdAsync(Guid ticketId);

        /// <summary>
        /// Retrieves a paginated list of use cases asynchronously based on the provided parameters.
        /// </summary>
        /// <param name="getUsecases">The parameters for retrieving use cases.</param>
        /// <returns>A task representing the asynchronous operation. The paginated list of use cases with detailed information.</returns>
        Task<PaginationResponseDto<UsecaseFullDto>> GetUsecasesAsync(GetUsecasesDto getUsecases);

        /// <summary>
        /// Retrieves a paginated list of use cases asynchronously based on the provided parameters.
        /// </summary>
        /// <param name="getUsecases">The parameters for retrieving use cases.</param>
        /// <returns>A task representing the asynchronous operation. The paginated list of use cases with detailed information.</returns>
        Task<UsecaseFullDto> UpdateUsecaseAsync(Guid usecaseId,  UsecaseDataFullDto usecase);
    }
}
