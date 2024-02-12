using AICommunicationService.RAG.Models;

namespace AICommunicationService.RAG.Interfaces;

public interface IDocumentSharingRepository
{
    Task AddDocumentSharingAsync(DocumentSharing documentSharing);
    Task DeleteDocumentSharingAsync(Guid userId, Guid documentId);
    Task DeleteAllDocumentSharingAsync(Guid documentId);
    Task<List<Guid>> GetAllSharedUsersAsync(Guid documentId);
}