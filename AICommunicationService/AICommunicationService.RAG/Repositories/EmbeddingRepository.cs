using AICommunicationService.RAG.Context.Vector;
using AICommunicationService.RAG.Models;

namespace AICommunicationService.RAG.Repositories
{
    public class EmbeddingRepository
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
    }
}
