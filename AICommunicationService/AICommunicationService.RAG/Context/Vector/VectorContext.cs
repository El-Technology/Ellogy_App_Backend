using AICommunicationService.Common;
using AICommunicationService.RAG.Models;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;


namespace AICommunicationService.RAG.Context.Vector
{
    public class VectorContext : DbContext
    {
        public DbSet<Document> Documents { get; set; } = null!;
        public DbSet<Embedding> Embeddings { get; set; } = null!;

        public VectorContext() { }
        public VectorContext(DbContextOptions<VectorContext> options) : base(options) { }

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
}
