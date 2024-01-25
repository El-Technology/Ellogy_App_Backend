using AICommunicationService.RAG.Models;

namespace AICommunicationService.RAG.Interfaces
{
    public interface IDocumentRepository
    {
        Task AddDocumentAsync(Document document);
        Task DeleteDocumentAsync(string documentName);
        Task<List<Document>> GetAllUserDocumentsAsync(Guid userId);
        Task<Document?> GetDocumentByNameAsync(string documentName);
        Task UpdateDocumentStatusAsync(string documentName, bool? status);
    }
}
