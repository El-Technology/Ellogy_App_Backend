using AICommunicationService.RAG.Models;

namespace AICommunicationService.RAG.Interfaces
{
    public interface IEmbeddingRepository
    {
        Task AddEmbeddingAsync(Embedding embedding);
        Task AddRangeEmbeddingsAsync(List<Embedding> embeddings);
        Task DeleteEmbeddingsAsync(string fileName);
        Task<Embedding?> GetTheClosestEmbeddingAsync(string fileName, float[] searchRequest);
    }
}
