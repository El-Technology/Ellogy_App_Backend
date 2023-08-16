using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManager.BLL.Dtos;
using UserManager.BLL.Interfaces;

namespace UserManager.Api.Controllers
{
    /// <summary>
    /// Controller for handling report-related operations.
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Endpoint for sending a report asynchronously.
        /// </summary>
        /// <remarks>
        /// This endpoint allows users to send a report by providing a <paramref name="reportModel"/>.
        /// </remarks>
        /// <param name="reportModel">The model containing the report information.</param>
        /// <returns>Returns an OK response if the report is sent successfully.</returns>
        [HttpPost]
        [Route("sendReport")]
        public async Task<IActionResult> SendReport(ReportModel reportModel)
        {
            await _reportService.SendReportAsync(reportModel);
            return Ok();
        }
    }
}
