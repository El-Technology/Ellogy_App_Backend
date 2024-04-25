using Microsoft.EntityFrameworkCore;
using TicketsManager.DAL.Models;
using TicketsManager.DAL.Models.UserStoryTests;

namespace TicketsManager.DAL.Context;

public class TicketsManagerDbContext : DbContext
{
    public TicketsManagerDbContext()
    {
    }

    public TicketsManagerDbContext(DbContextOptions<TicketsManagerDbContext> options) : base(options)
    {
    }

    public DbSet<UserStoryTest> UserStoryTests { get; set; } = null!;
    public DbSet<TestCase> TestCases { get; set; } = null!;
    public DbSet<TestPlan> TestPlans { get; set; } = null!;
    public DbSet<ActionHistory> ActionHistories { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<Usecase> Usecases { get; set; } = null!;
    public DbSet<TicketTable> TicketTables { get; set; } = null!;
    public DbSet<TicketDiagram> TicketDiagrams { get; set; } = null!;
    public DbSet<Ticket> Tickets { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<TicketSummary> TicketSummaries { get; set; } = null!;
    public DbSet<SummaryScenario> SummaryScenarios { get; set; } = null!;
    public DbSet<SummaryAcceptanceCriteria> SummaryAcceptanceCriteria { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured) optionsBuilder.UseNpgsql();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TicketsManagerDbContext).Assembly);
    }
}