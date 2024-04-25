using AICommunicationService.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;

namespace AICommunicationService.DAL.Context;

public class AICommunicationContext : DbContext
{
    public DbSet<AIPrompt> AIPrompts { get; set; } = null!;
    public DbSet<Document> Documents { get; set; } = null!;
    public DbSet<Embedding> Embeddings { get; set; } = null!;
    public DbSet<DocumentSharing> DocumentSharing { get; set; } = null!;

    public AICommunicationContext() { }
    public AICommunicationContext(DbContextOptions<AICommunicationContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseNpgsql(o => o.UseVector());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AICommunicationContext).Assembly);
    }
}
