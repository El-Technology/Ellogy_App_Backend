using AICommunicationService.RAG.Models;

namespace AICommunicationService.RAG.Interfaces
{
    public interface IDocumentRepository
    {
        Task AddDocumentAsync(Document document);
    }
}
