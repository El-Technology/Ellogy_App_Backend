using AICommunicationService.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AICommunicationService.DAL.Context.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name)
            .IsRequired();
        builder.Property(e => e.UserId)
            .IsRequired();
        builder.Property(e => e.IsReadyToUse);
        builder.Property(e => e.CreationDate);

        builder.HasMany(e => e.Embeddings)
            .WithOne(e => e.Document);
        builder.HasMany(e => e.DocumentSharing)
            .WithOne(e => e.Document);
    }
}