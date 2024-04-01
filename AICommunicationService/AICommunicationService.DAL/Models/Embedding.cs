using Pgvector;
using System.ComponentModel.DataAnnotations.Schema;

namespace AICommunicationService.DAL.Models;

public class Embedding
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;

    [Column(TypeName = "vector(1536)")]
    public Vector? Vector { get; set; }

    public Guid DocumentId { get; set; }
    public Document Document { get; set; }
}
