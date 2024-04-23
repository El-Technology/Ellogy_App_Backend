using Microsoft.EntityFrameworkCore;
using PaymentManager.DAL.Models;

namespace PaymentManager.DAL.Context.PaymentContext;

public class PaymentContext : DbContext
{
    public PaymentContext()
    {
    }

    public PaymentContext(DbContextOptions<PaymentContext> options) : base(options)
    {
    }

    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<Wallet> Wallets { get; set; } = null!;
    public DbSet<Subscription> Subscriptions { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured) optionsBuilder.UseNpgsql();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentContext).Assembly);
    }
}