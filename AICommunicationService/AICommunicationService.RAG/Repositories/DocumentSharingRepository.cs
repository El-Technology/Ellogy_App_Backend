using AICommunicationService.RAG.Context.Vector;
using AICommunicationService.RAG.Models;
using Microsoft.EntityFrameworkCore;

namespace AICommunicationService.RAG.Repositories;

public class DocumentSharingRepository
{
    private readonly VectorContext _context;

    public DocumentSharingRepository(VectorContext context)
    {
        _context = context;
    }

    public async Task AddDocumentSharingAsync(DocumentSharing documentSharing)
    {
        await _context.DocumentSharing.AddAsync(documentSharing);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteDocumentSharingAsync(Guid userId, Guid documentId)
    {
        await _context.DocumentSharing
            .Where(a => a.UserId == userId && a.DocumentId == documentId)
            .ExecuteDeleteAsync();
    }

    public async Task<List<Guid>> GetAllSharedUsersAsync(Guid documentId)
    {
        return await _context.DocumentSharing
            .Where(a => a.DocumentId == documentId)
            .Select(a => a.UserId)
            .ToListAsync();
    }
}