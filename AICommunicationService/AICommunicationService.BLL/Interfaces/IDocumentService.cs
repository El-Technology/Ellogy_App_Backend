namespace AICommunicationService.BLL.Interfaces
{
    public interface IDocumentService
    {
        string GetDeleteFileUrl(string fileName);
        string GetFileUrl(string fileName);
        Task<string> GetTheClosesContextAsync(string searchRequest, string fileName);
        string GetUploadFileUrl(string fileName);
        Task InsertDocumentContextInVectorDbAsync(string fileName, Guid userId);
        Task<string> ReadPdf(string fileName);
    }
}
