using AICommunicationService.Common;
using AICommunicationService.RAG.Models;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;

namespace AICommunicationService.RAG.Context.Vector;

public class VectorContext : DbContext
{
    public VectorContext()
    {
    }

    public VectorContext(DbContextOptions<VectorContext> options) : base(options)
    {
    }

    public DbSet<Document> Documents { get; set; } = null!;
    public DbSet<Embedding> Embeddings { get; set; } = null!;
    public DbSet<DocumentSharing> DocumentSharing { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(EnvironmentVariables.ConnectionStringVector, o => o.UseVector());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VectorContext).Assembly);
    }
}