using Microsoft.EntityFrameworkCore;
using TicketsManager.Common;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Context;

public class TicketsManagerDbContext : DbContext
{
    public DbSet<Ticket> Tickets { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<TicketSummary> TicketSummaries { get; set; } = null!;

    public TicketsManagerDbContext() { }

    public TicketsManagerDbContext(DbContextOptions<TicketsManagerDbContext> options) : base(options)
    { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(EnvironmentVariables.ConnectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TicketsManagerDbContext).Assembly);
    }
}
