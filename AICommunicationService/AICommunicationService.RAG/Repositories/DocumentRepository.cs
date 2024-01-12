using AICommunicationService.RAG.Context.Vector;
using AICommunicationService.RAG.Interfaces;
using AICommunicationService.RAG.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<string>> GetAllUserDocumentsAsync(Guid userId)
        {
            return await _context.Documents
                .Where(a => a.UserId == userId)
                .Select(a => a.Name)
                .ToListAsync();
        }
    }
}
