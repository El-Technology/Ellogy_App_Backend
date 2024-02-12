namespace AICommunicationService.RAG.Models;

public class Document
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public bool? IsReadyToUse { get; set; } = null;
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;

    public IEnumerable<Embedding> Embeddings { get; set; } = new List<Embedding>();
    public IEnumerable<DocumentSharing> DocumentSharing { get; set; } = new List<DocumentSharing>();
}