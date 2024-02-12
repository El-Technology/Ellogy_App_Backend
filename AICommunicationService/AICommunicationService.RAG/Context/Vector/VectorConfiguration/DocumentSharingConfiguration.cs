using AICommunicationService.RAG.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AICommunicationService.RAG.Context.Vector.VectorConfiguration;

public class DocumentSharingConfiguration : IEntityTypeConfiguration<DocumentSharing>
{
    public void Configure(EntityTypeBuilder<DocumentSharing> builder)
    {
        builder.ToTable("DocumentSharing");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.UserId)
            .IsRequired();
        builder.Property(e => e.DocumentId)
            .IsRequired();
        builder.HasOne(e => e.Document)
            .WithMany(e => e.DocumentSharing);
    }
}