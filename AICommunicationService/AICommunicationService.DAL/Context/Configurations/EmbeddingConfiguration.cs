using AICommunicationService.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AICommunicationService.DAL.Context.Configurations;

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
