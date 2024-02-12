using AICommunicationService.RAG.Context.Vector;
using AICommunicationService.RAG.Interfaces;
using AICommunicationService.RAG.Models;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace AICommunicationService.RAG.Repositories;

public class EmbeddingRepository : IEmbeddingRepository
{
    private readonly VectorContext _context;

    public EmbeddingRepository(VectorContext context)
    {
        _context = context;
    }

    public async Task AddRangeEmbeddingsAsync(List<Embedding> embeddings)
    {
        await _context.Embeddings.AddRangeAsync(embeddings);
        await _context.SaveChangesAsync();
    }

    public async Task AddEmbeddingAsync(Embedding embedding)
    {
        await _context.Embeddings.AddAsync(embedding);
        await _context.SaveChangesAsync();
    }

    public async Task<Embedding?> GetTheClosestEmbeddingAsync(Guid userId, string fileName, float[] searchRequest)
    {
        return await _context.Embeddings
            .Where(a => a.Document.Name.Equals(fileName) && a.Document.UserId == userId)
            .OrderBy(a => a.Vector!.L2Distance(new Vector(searchRequest)))
            .Take(1)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> CheckIfEmbeddingAlreadyExistAsync(Guid userId, string fileName)
    {
        return await _context.Embeddings
            .Where(a => a.Document.Name.Equals(fileName) && a.Document.UserId == userId)
            .AnyAsync();
    }

    public async Task DeleteEmbeddingsAsync(Guid userId, string fileName)
    {
        await _context.Embeddings
            .Where(a => a.Document.Name.Equals(fileName) && a.Document.UserId == userId)
            .ExecuteDeleteAsync();
    }
}