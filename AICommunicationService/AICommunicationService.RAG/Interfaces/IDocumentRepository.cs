using AICommunicationService.RAG.Models;

namespace AICommunicationService.RAG.Interfaces;

public interface IDocumentRepository
{
    Task AddDocumentAsync(Document document);
    Task DeleteDocumentAsync(Guid userId, string documentName);
    Task<List<Document>> GetAllUserDocumentsAsync(Guid userId);
    Task<Document?> GetDocumentByNameAsync(Guid userId, string documentName);
    Task UpdateDocumentStatusAsync(Guid userId, string documentName, bool? status);
}