using UserManager.Common.Models;

namespace UserManager.BLL.Interfaces
{
    public interface IReportService
    {
        Task SendReportAsync(ReportModel reportModel);
    }
}
