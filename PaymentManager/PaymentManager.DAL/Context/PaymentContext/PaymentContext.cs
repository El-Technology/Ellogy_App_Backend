using Microsoft.EntityFrameworkCore;
using PaymentManager.Common;
using PaymentManager.DAL.Models;

namespace PaymentManager.DAL.Context.PaymentContext
{
    public class PaymentContext : DbContext
    {
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<Wallet> Wallets { get; set; } = null!;
        public DbSet<Subscription> Subscriptions { get; set; } = null!;

        public PaymentContext() { }
        public PaymentContext(DbContextOptions<PaymentContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(EnvironmentVariables.ConnectionStringPayment);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentContext).Assembly);
        }
    }
}
