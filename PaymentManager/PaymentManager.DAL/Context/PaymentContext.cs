using Microsoft.EntityFrameworkCore;
using PaymentManager.DAL.Models;

namespace PaymentManager.DAL.Context
{
    public class PaymentContext : DbContext
    {
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<Wallet> Walets { get; set; } = null!;

        public PaymentContext() { }
        public PaymentContext(DbContextOptions<PaymentContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTIONSTRING_PAYMENT"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentContext).Assembly);
        }
    }
}
