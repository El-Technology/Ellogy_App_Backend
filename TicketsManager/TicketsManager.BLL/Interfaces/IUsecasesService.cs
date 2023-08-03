using TicketsManager.BLL.Dtos.TicketVisualizationDtos.FullDtos;
using TicketsManager.BLL.Dtos.TicketVisualizationDtos.UpdateDtos;
using TicketsManager.BLL.Dtos.TicketVisualizationDtos.UsecasesDtos;
using TicketsManager.Common.Dtos;

namespace TicketsManager.BLL.Interfaces
{
    /// <summary>
    /// Interface for managing use cases related operations.
    /// </summary>
    public interface IUsecasesService
    {
        /// <summary>
        /// Create use cases by processing the provided data.
        /// </summary>
        /// <param name="createUsecasesDto">The data containing models of diagrams and tables for use cases.</param>
        /// <returns>Returns a response containing details of the created use cases.</returns>
        Task<CreateUsecasesResponseDto> CreateUsecasesAsync(CreateUsecasesDto createUsecasesDto);

        /// <summary>
        /// Retrieve diagrams for use cases based on the specified query parameters.
        /// </summary>
        /// <param name="getDiagramsDto">The query parameters for filtering diagrams.</param>
        /// <returns>Returns a paginated response containing the requested diagrams for use cases.</returns>
        Task<PaginationResponseDto<TicketDiagramFullDto>> GetDiagramsAsync(GetDiagramsDto getDiagramsDto);

        /// <summary>
        /// Retrieve a table for the specified ticket.
        /// </summary>
        /// <param name="ticketId">The unique identifier of the ticket for which the table is requested.</param>
        /// <returns>Returns the requested table data in the response.</returns>
        Task<TicketTableFullDto> GetTableAsync(Guid ticketId);

        /// <summary>
        /// Update the diagram for the specified ticket diagram ID.
        /// </summary>
        /// <param name="ticketDiagramId">The unique identifier of the ticket diagram to be updated.</param>
        /// <param name="ticketDiagram">The updated data for the ticket diagram.</param>
        /// <returns>Returns the updated ticket diagram data in the response.</returns>
        Task<TicketDiagramFullDto> UpdateTicketDiagram(Guid ticketDiagramId, TicketDiagramUpdateDto ticketDiagram);

        /// <summary>
        /// Update the table for the specified ticket table ID.
        /// </summary>
        /// <param name="ticketTableId">The unique identifier of the ticket table to be updated.</param>
        /// <param name="ticketTable">The updated data for the ticket table.</param>
        /// <returns>Returns the updated ticket table data in the response.</returns>
        Task<TicketTableFullDto> UpdateTicketTable(Guid ticketTableId, TicketTableUpdateDto ticketTable);
    }
}
