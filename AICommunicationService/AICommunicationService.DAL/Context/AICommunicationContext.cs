using AICommunicationService.DAL.Models;
using Microsoft.EntityFrameworkCore;
using TicketsManager.Common;

namespace AICommunicationService.DAL.Context
{
    public class AICommunicationContext : DbContext
    {
        public DbSet<AIPrompt> AIPrompts { get; set; } = null!;
        public AICommunicationContext() { }
        public AICommunicationContext(DbContextOptions<AICommunicationContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(EnvironmentVariables.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AICommunicationContext).Assembly);
        }
    }
}
