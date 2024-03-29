using UserManager.BLL.Dtos.ReportDto;

namespace UserManager.BLL.Interfaces
{
    /// <summary>
    /// Interface for sending report-related operations.
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// Sends a report asynchronously using the provided <paramref name="reportModel"/>.
        /// </summary>
        /// <param name="reportModel">The model containing the information for the report.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendReportAsync(ReportModel reportModel);
    }
}
