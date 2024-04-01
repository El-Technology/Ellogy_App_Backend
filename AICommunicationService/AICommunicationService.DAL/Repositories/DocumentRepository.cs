using AICommunicationService.Common.Dtos;
using AICommunicationService.DAL.Context;
using AICommunicationService.DAL.Extensions;
using AICommunicationService.DAL.Interfaces;
using AICommunicationService.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace AICommunicationService.DAL.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly AICommunicationContext _context;

    public DocumentRepository(AICommunicationContext context)
    {
        _context = context;
    }

    public async Task AddDocumentAsync(Document document)
    {
        await _context.Documents.AddAsync(document);
        await _context.SaveChangesAsync();
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

    public async Task<PaginationResponseDto<Document>> GetAllUserDocumentsAsync(Guid userId,
        PaginationRequestDto paginationRequest)
    {
        return await _context.Documents
            .Where(a => a.UserId == userId || a.DocumentSharing.Any(sharing => sharing.UserId == userId))
            .GetDocumentsPaginatedResult(paginationRequest);
    }
}