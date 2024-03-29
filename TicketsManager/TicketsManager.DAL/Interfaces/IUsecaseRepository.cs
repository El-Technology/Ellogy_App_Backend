using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Interfaces;

/// <summary>
/// Interface for accessing and managing use case-related data in the repository.
/// </summary>
public interface IUsecaseRepository
{
    /// <summary>
    /// Creates a list of use cases asynchronously.
    /// </summary>
    /// <param name="usecases">The list of use cases to be created.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateUsecasesAsync(List<Usecase> usecases);

    /// <summary>
    /// Deletes all usecases by ticket id
    /// </summary>
    /// <param name="ticketId">ID of the ticket, under which all use cases are located.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteUsecasesAsync(Guid ticketId);
    IQueryable<TicketSummary> GetTicketSummariesByIdsAsync(List<Guid> ticketSummaryIds);

    /// <summary>
    /// Retrieves a use case by its ID asynchronously.
    /// </summary>
    /// <param name="usecaseId">The ID of the use case to retrieve.</param>
    /// <returns>A task representing the asynchronous operation. The retrieved use case or null if not found.</returns>
    Task<Usecase?> GetUsecaseByIdAsync(Guid usecaseId);

    /// <summary>
    /// Retrieves a paginated list of use cases asynchronously based on the specified ticket ID.
    /// </summary>
    /// <param name="paginationRequest">The pagination parameters.</param>
    /// <param name="ticketId">The ID of the ticket associated with the use cases.</param>
    /// <returns>A task representing the asynchronous operation. The paginated list of use cases.</returns>
    Task<PaginationResponseDto<Usecase>> GetUsecasesAsync(PaginationRequestDto paginationRequest, Guid ticketId);
    Task<Guid> GetUserIdByTicketIdAsync(Guid ticketId);

    /// <summary>
    /// Updates a use case asynchronously.
    /// </summary>
    /// <param name="usecase">The use case to be updated.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateUsecaseAsync(Usecase usecase);
}
