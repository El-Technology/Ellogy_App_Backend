namespace AICommunicationService.RAG.Models
{
    public class Document
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid UserId { get; set; }

        public IEnumerable<Embedding> Embeddings { get; set; } = new List<Embedding>();
    }
}
