using AICommunicationService.DAL.Context.AiCommunication;
using AICommunicationService.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace AICommunicationService.DAL.Context.Wallets;

public class WalletContext : DbContext
{
    public DbSet<Wallet> Wallets { get; set; } = null!;

    public WalletContext() { }
    public WalletContext(DbContextOptions<WalletContext> options) : base(options) { }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AICommunicationContext).Assembly);
    }
}
