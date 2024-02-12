using AICommunicationService.RAG.Models;

namespace AICommunicationService.RAG.Interfaces;

public interface IEmbeddingRepository
{
    Task AddEmbeddingAsync(Embedding embedding);
    Task AddRangeEmbeddingsAsync(List<Embedding> embeddings);
    Task<bool> CheckIfEmbeddingAlreadyExistAsync(Guid userId, string fileName);
    Task DeleteEmbeddingsAsync(Guid userId, string fileName);
    Task<Embedding?> GetTheClosestEmbeddingAsync(Guid userId, string fileName, float[] searchRequest);
}