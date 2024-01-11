using AICommunicationService.RAG.Context.Vector;
using AICommunicationService.RAG.Interfaces;
using AICommunicationService.RAG.Models;

namespace AICommunicationService.RAG.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly VectorContext _context;
        public DocumentRepository(VectorContext context)
        {
            _context = context;
        }

        public async Task AddDocumentAsync(Document document)
        {
            await _context.Documents.AddAsync(document);
            await _context.SaveChangesAsync();
        }
    }
}
