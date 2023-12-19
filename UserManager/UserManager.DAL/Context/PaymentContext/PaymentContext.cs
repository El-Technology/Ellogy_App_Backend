using Microsoft.EntityFrameworkCore;
using UserManager.Common;
using UserManager.DAL.Models;

namespace UserManager.DAL.Context.PaymentContext
{
    public class PaymentContext : DbContext
    {
        public DbSet<Wallet> Wallets { get; set; } = null!;

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
