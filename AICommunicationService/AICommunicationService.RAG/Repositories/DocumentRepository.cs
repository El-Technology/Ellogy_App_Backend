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

        public async Task<List<Document>> GetAllUserDocumentsAsync(Guid userId)
        {
            return await _context.Documents
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<Document?> GetDocumentByNameAsync(string documentName)
        {
            return await _context.Documents
                .FirstOrDefaultAsync(a => a.Name.Equals(documentName));

        }

        public async Task DeleteDocumentAsync(string documentName)
        {
            await _context.Documents
                .Where(a => a.Name.Equals(documentName))
                .ExecuteDeleteAsync();
        }

        public async Task UpdateDocumentStatusAsync(string documentName, bool? status)
        {
            await _context.Documents
                .Where(a => a.Name.Equals(documentName))
                .ExecuteUpdateAsync(a => a.SetProperty(a => a.IsReadyToUse, a => status));
        }
    }
}
