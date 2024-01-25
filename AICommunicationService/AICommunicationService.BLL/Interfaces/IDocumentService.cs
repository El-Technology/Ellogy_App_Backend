
using AICommunicationService.BLL.Dtos;

namespace AICommunicationService.BLL.Interfaces
{
    public interface IDocumentService
    {
        Task CheckIfDocumentWasUploadedAsync(Guid userId, string fileName);
        Task<List<DocumentResponseDto>> GetAllUserDocumentsAsync(Guid userId);
        Task DeleteFileAsync(Guid userId, string fileName);
        string GetFileUrl(string fileName);
        Task<string> GetTheClosesContextAsync(string searchRequest, string fileName);
        string GetUploadFileUrl(string fileName);
        Task InsertDocumentContextInVectorDbAsync(string fileName, Guid userId);
        Task<string> ReadPdf(string fileName);
    }
}
