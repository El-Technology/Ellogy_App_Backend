using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Interfaces
{
    /// <summary>
    /// Interface for accessing and managing use case-related data in the repository.
    /// </summary>
    public interface IUsecaseRepository
    {
        /// <summary>
        /// Create use cases by persisting the provided table and diagram data in the repository.
        /// </summary>
        /// <param name="table">The table data to be created for use cases.</param>
        /// <param name="diagrams">The list of diagram data to be associated with the use cases.</param>
        /// <returns>Returns a task representing the asynchronous create operation.</returns>
        Task CreateUsecasesAsync(TicketTable table, List<TicketDiagram> diagrams);

        /// <summary>
        /// Retrieve diagrams associated with a specific ticket based on the provided pagination request.
        /// </summary>
        /// <param name="paginationRequest">The pagination configuration for the result.</param>
        /// <param name="ticketId">The unique identifier of the ticket for which diagrams are requested.</param>
        /// <returns>Returns a paginated response containing the requested diagrams for the ticket.</returns>
        Task<PaginationResponseDto<TicketDiagram>> GetDiagramsAsync(PaginationRequestDto paginationRequest, Guid ticketId);

        /// <summary>
        /// Retrieve the table associated with a specific ticket.
        /// </summary>
        /// <param name="ticketId">The unique identifier of the ticket for which the table is requested.</param>
        /// <returns>Returns the table data associated with the ticket, or null if not found.</returns>
        Task<TicketTable?> GetTableAsync(Guid ticketId);

        /// <summary>
        /// Retrieve the table with the specified tableId.
        /// </summary>
        /// <param name="tableId">The unique identifier of the table to be retrieved.</param>
        /// <returns>Returns the table data, or null if not found.</returns>
        Task<TicketTable?> GetTableByIdAsync(Guid tableId);

        /// <summary>
        /// Retrieve the diagram with the specified diagramId.
        /// </summary>
        /// <param name="diagramId">The unique identifier of the diagram to be retrieved.</param>
        /// <returns>Returns the diagram data, or null if not found.</returns>
        Task<TicketDiagram?> GetDiagramByIdAsync(Guid diagramId);

        /// <summary>
        /// Update the specified diagram in the repository.
        /// </summary>
        /// <param name="ticketDiagram">The updated diagram data.</param>
        /// <returns>Returns a task representing the asynchronous update operation.</returns>
        Task UpdateDiagramAsync(TicketDiagram ticketDiagram);

        /// <summary>
        /// Update the specified table in the repository.
        /// </summary>
        /// <param name="ticketTable">The updated table data.</param>
        /// <returns>Returns a task representing the asynchronous update operation.</returns>
        Task UpdateTableAsync(TicketTable ticketTable);
    }
}
