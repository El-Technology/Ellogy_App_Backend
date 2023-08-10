using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketsManager.BLL.Dtos.TicketUsecaseDtos.FullDtos;
using TicketsManager.BLL.Dtos.TicketUsecaseDtos.UsecasesDtos;

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
        public async Task<IActionResult> CreateUsecases([FromBody] List<CreateUsecasesDto> createUsecasesDto)
        {
            var tickets = await _usecasesService.CreateUsecasesAsync(createUsecasesDto);
            return Ok(tickets);
        }

        /// <summary>
        /// Endpoint to retrieve use cases.
        /// </summary>
        /// <remarks>
        /// This endpoint allows users to get use cases by providing a <paramref name="getUsecases"/> containing query parameters to filter the results.
        /// </remarks>
        /// <param name="getUsecases">Query parameters for filtering diagrams.</param>
        /// <returns>Returns the requested use cases in the response.</returns>
        [HttpPost]
        [Route("get")]
        public async Task<IActionResult> GetUsecases([FromBody] GetUsecasesDto getUsecases)
        {
            var tickets = await _usecasesService.GetUsecasesAsync(getUsecases);
            return Ok(tickets);
        }

        /// <summary>
        /// Controller endpoint to update a use case.
        /// </summary>
        /// <remarks>
        /// This endpoint allows users to update a specific use case by providing the use case ID and the updated data in the request body.
        /// </remarks>
        /// <param name="usecaseId">The ID of the use case to update.</param>
        /// <param name="usecase">The updated data for the use case.</param>
        /// <returns>Returns the updated use case in the response.</returns>
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateTable(Guid usecaseId, [FromBody] UsecaseDataFullDto usecase)
        {
            var response = await _usecasesService.UpdateUsecaseAsync(usecaseId, usecase);
            return Ok(response);
        }

        /// <summary>
        /// Controller endpoint to delete a usecases.
        /// </summary>
        /// <remarks>
        /// This endpoint allows users to deletes all usecases by providing the ticket ID.
        /// </remarks>
        /// <param name="ticketId">The ID of the ticket.</param>
        /// <returns>Returns the updated use case in the response.</returns>
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteUsecases(Guid ticketId)
        {
            await _usecasesService.DeleteUsecasesByTicketIdAsync(ticketId);
            return Ok("Successfully deleted");
        }
    }
}
