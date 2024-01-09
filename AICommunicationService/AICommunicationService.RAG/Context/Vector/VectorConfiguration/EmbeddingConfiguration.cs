using AICommunicationService.RAG.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AICommunicationService.RAG.Context.Vector.VectorConfiguration
{
    public class EmbeddingConfiguration : IEntityTypeConfiguration<Embedding>
    {
        public void Configure(EntityTypeBuilder<Embedding> builder)
        {
            builder.ToTable("Embeddings");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.DocumentId);
            builder.Property(e => e.Vector);
            builder.Property(e => e.Text);

            builder.HasOne(e => e.Document)
                .WithMany(e => e.Embeddings);
        }
    }
}
