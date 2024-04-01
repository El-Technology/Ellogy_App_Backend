using AICommunicationService.DAL.Context;
using AICommunicationService.DAL.Interfaces;
using AICommunicationService.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace AICommunicationService.DAL.Repositories;

public class DocumentSharingRepository : IDocumentSharingRepository
{
    private readonly AICommunicationContext _context;

    public DocumentSharingRepository(AICommunicationContext context)
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

    public async Task DeleteAllDocumentSharingAsync(Guid documentId)
    {
        await _context.DocumentSharing
            .Where(a => a.DocumentId == documentId)
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