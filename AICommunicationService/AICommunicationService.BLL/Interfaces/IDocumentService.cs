
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
        Task<string> GetUploadFileUrlAsync(string fileName);
        Task<DocumentResponseDto> InsertDocumentContextInVectorDbAsync(string fileName, Guid userId);
        Task<string> ReadPdf(string fileName);
    }
}
