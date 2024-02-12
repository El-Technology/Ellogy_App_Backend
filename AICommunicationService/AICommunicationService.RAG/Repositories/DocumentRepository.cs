using AICommunicationService.RAG.Context.Vector;
using AICommunicationService.RAG.Interfaces;
using AICommunicationService.RAG.Models;
using Microsoft.EntityFrameworkCore;

namespace AICommunicationService.RAG.Repositories;

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
            .Where(a => a.UserId == userId || a.DocumentSharing.Any(sharing => sharing.UserId == userId))
            .ToListAsync();
    }

    public async Task<Document?> GetDocumentByNameAsync(Guid userId, string documentName)
    {
        return await _context.Documents
            .FirstOrDefaultAsync(a => a.Name.Equals(documentName) && a.UserId == userId);
    }

    public async Task UpdateDocumentStatusAsync(Guid userId, string documentName, bool? status)
    {
        await _context.Documents
            .Where(a => a.Name.Equals(documentName) && a.UserId == userId)
            .ExecuteUpdateAsync(a => a.SetProperty(a => a.IsReadyToUse, a => status));
    }

    public async Task DeleteDocumentAsync(Guid userId, string documentName)
    {
        await _context.Documents
            .Where(a => a.Name.Equals(documentName) && a.UserId == userId)
            .ExecuteDeleteAsync();
    }
}