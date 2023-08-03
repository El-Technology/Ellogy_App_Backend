using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketsManager.BLL.Dtos.TicketVisualizationDtos.UpdateDtos;
using TicketsManager.BLL.Dtos.TicketVisualizationDtos.UsecasesDtos;
using TicketsManager.BLL.Interfaces;

namespace TicketsManager.Api.Controllers
{
    /// <summary>
    /// Represents the API endpoints for managing usecases.
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UsecasesController : ControllerBase
    {
        private readonly IUsecasesService _usecasesService;
        public UsecasesController(IUsecasesService usecasesService)
        {
            _usecasesService = usecasesService;
        }

        /// <summary>
        /// Endpoint to create diagrams and tables in use cases.
        /// </summary>
        /// <remarks>
        /// This endpoint allows users to create diagrams and tables for use cases by providing a <paramref name="createUsecasesDto"/> containing models of diagrams and tables.
        /// </remarks>
        /// <param name="createUsecasesDto">The data required to create diagrams and tables for use cases.</param>
        /// <returns>Returns the created use cases along with their details in the response.</returns>
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateUsecases([FromBody] CreateUsecasesDto createUsecasesDto)
        {
            var tickets = await _usecasesService.CreateUsecasesAsync(createUsecasesDto);
            return Ok(tickets);
        }

        /// <summary>
        /// Endpoint to retrieve diagrams for use cases.
        /// </summary>
        /// <remarks>
        /// This endpoint allows users to get diagrams for use cases by providing a <paramref name="getDiagramsDto"/> containing query parameters to filter the results.
        /// </remarks>
        /// <param name="getDiagramsDto">Query parameters for filtering diagrams.</param>
        /// <returns>Returns the requested diagrams for use cases in the response.</returns>
        [HttpPost]
        [Route("getDiagrams")]
        public async Task<IActionResult> GetDiagrams([FromBody] GetDiagramsDto getDiagramsDto)
        {
            var tickets = await _usecasesService.GetDiagramsAsync(getDiagramsDto);
            return Ok(tickets);
        }

        /// <summary>
        /// Endpoint to retrieve a table for a specific ticket.
        /// </summary>
        /// <remarks>
        /// This endpoint allows users to get a table associated with the provided <paramref name="ticketId"/>.
        /// </remarks>
        /// <param name="ticketId">The unique identifier of the ticket for which the table is requested.</param>
        /// <returns>Returns the requested table data in the response.</returns>
        [HttpGet]
        [Route("getTable")]
        public async Task<IActionResult> GetTable(Guid ticketId)
        {
            var tickets = await _usecasesService.GetTableAsync(ticketId);
            return Ok(tickets);
        }

        /// <summary>
        /// Endpoint to update a table for a specific ticket.
        /// </summary>
        /// <remarks>
        /// This endpoint allows users to update the table associated with the provided <paramref name="tableId"/> using the data in <paramref name="ticketTable"/>.
        /// </remarks>
        /// <param name="tableId">The unique identifier of the table to be updated.</param>
        /// <param name="ticketTable">The updated table data.</param>
        /// <returns>Returns the response indicating the success of the update operation.</returns>
        [HttpPut]
        [Route("updateTable")]
        public async Task<IActionResult> UpdateTable(Guid tableId, [FromBody] TicketTableUpdateDto ticketTable)
        {
            var response = await _usecasesService.UpdateTicketTable(tableId, ticketTable);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint to update a diagram for a specific ticket.
        /// </summary>
        /// <remarks>
        /// This endpoint allows users to update the diagram associated with the provided <paramref name="diagramId"/> using the data in <paramref name="ticketDiagram"/>.
        /// </remarks>
        /// <param name="diagramId">The unique identifier of the diagram to be updated.</param>
        /// <param name="ticketDiagram">The updated diagram data.</param>
        /// <returns>Returns the response indicating the success of the update operation.</returns>
        [HttpPut]
        [Route("updateDiagram")]
        public async Task<IActionResult> UpdateDiagram(Guid diagramId, [FromBody] TicketDiagramUpdateDto ticketDiagram)
        {
            var response = await _usecasesService.UpdateTicketDiagram(diagramId, ticketDiagram);
            return Ok(response);
        }
    }
}
