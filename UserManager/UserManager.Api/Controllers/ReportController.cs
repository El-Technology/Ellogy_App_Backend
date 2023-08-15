using Microsoft.AspNetCore.Mvc;
using UserManager.BLL.Interfaces;
using UserManager.Common.Models;

namespace UserManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost]
        [Route("sendReport")]
        public async Task<IActionResult> SendReport(ReportModel reportModel)
        {
            await _reportService.SendReportAsync(reportModel);
            return Ok();
        }
    }
}
