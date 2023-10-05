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
        /// <param name="userIdFromToken">User id taken from jwt token</param>
        /// <returns>A task representing the asynchronous operation. The response containing information about the created use cases.</returns>
        Task<CreateUsecasesResponseDto> CreateUsecasesAsync(List<CreateUsecasesDto> createUsecasesDto, Guid userIdFromToken);

        /// <summary>
        /// Deletes all usecases by ticket id
        /// </summary>
        /// <param name="ticketId">ID of the ticket, under which all use cases are located.</param>
        /// <param name="userIdFromToken">User id taken from jwt token</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteUsecasesByTicketIdAsync(Guid ticketId, Guid userIdFromToken);

        /// <summary>
        /// Retrieves a paginated list of use cases asynchronously based on the provided parameters.
        /// </summary>
        /// <param name="getUsecases">The parameters for retrieving use cases.</param>
        /// <param name="userIdFromToken">User id taken from jwt token</param>
        /// <returns>A task representing the asynchronous operation. The paginated list of use cases with detailed information.</returns>
        Task<PaginationResponseDto<UsecaseFullDto>> GetUsecasesAsync(GetUsecasesDto getUsecases, Guid userIdFromToken);

        /// <summary>
        /// Update specific usecase.
        /// </summary>
        /// <param name="usecaseId">Usecase id</param>
        /// <param name="usecase">Model with field for update</param>
        /// <param name="userIdFromToken">User id taken from jwt token</param>
        /// <returns>A task representing the asynchronous operation. The paginated list of use cases with detailed information.</returns>
        Task<UsecaseFullDto> UpdateUsecaseAsync(Guid usecaseId, UsecaseDataFullDto usecase, Guid userIdFromToken);
    }
}
