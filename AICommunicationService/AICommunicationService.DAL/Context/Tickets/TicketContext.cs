using AICommunicationService.DAL.Context.AiCommunication;
using AICommunicationService.DAL.Models;
using Microsoft.EntityFrameworkCore;
using TicketsManager.Common;

namespace AICommunicationService.DAL.Context.Tickets
{
    public class TicketContext : DbContext
    {
        public DbSet<Ticket> Tickets { get; set; } = null!;
        public TicketContext() { }
        public TicketContext(DbContextOptions<TicketContext> options) : base(options) { }

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
